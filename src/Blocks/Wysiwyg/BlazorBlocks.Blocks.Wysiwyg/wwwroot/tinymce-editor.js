let loading = null;

function loadTinyMce(scriptUrl) {
    if (window.tinymce) {
        return Promise.resolve(window.tinymce);
    }

    loading ??= new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = scriptUrl;
        script.referrerPolicy = 'origin';
        script.onload = () => resolve(window.tinymce);
        script.onerror = () => {
            loading = null;
            reject(new Error(`Could not load TinyMCE from ${scriptUrl}`));
        };
        document.head.appendChild(script);
    });

    return loading;
}

// TinyMCE adds sibling nodes next to its target, which Blazor's diffing would not expect inside
// a component-rendered element, so the textarea is created here, inside an element Blazor leaves empty.
export async function create(host, dotNetRef, html, options) {
    const tinymce = await loadTinyMce(options.scriptUrl);

    const textarea = document.createElement('textarea');
    textarea.value = html ?? '';
    host.appendChild(textarea);

    // In an expanded (near-fullscreen) dialog the editor fills the window instead of using the fixed height.
    // The offset leaves room for the dialog margin, its header, padding and the block's own heading.
    const expanded = host.closest('.bb-dialog--expanded') !== null;
    if (expanded) {
        host.style.height = 'calc(100vh - 14.5rem)';
    }

    const [editor] = await tinymce.init({
        target: textarea,
        license_key: options.licenseKey,
        height: expanded ? '100%' : options.height,
        // A modal <dialog> makes everything outside it inert, so menus and dialogs must not mount under document.body.
        ui_container: host.closest('dialog') ?? undefined,
        menubar: false,
        branding: false,
        promotion: false,
        plugins: 'lists link image table code autolink',
        toolbar: 'undo redo | blocks | bold italic underline strikethrough | forecolor backcolor | ' +
            'alignleft aligncenter alignright | bullist numlist outdent indent | link image table | removeformat code',
        setup: ed => {
            let timer = null;
            const push = () => dotNetRef.invokeMethodAsync('OnContentChanged', ed.getContent());

            ed.on('input change undo redo', () => {
                clearTimeout(timer);
                timer = setTimeout(push, 250);
            });
            ed.on('blur', () => {
                clearTimeout(timer);
                push();
            });
            // A pending debounce must not fire after the component (and its DotNetObjectReference) is gone.
            ed.on('remove', () => clearTimeout(timer));
        }
    });

    return {
        dispose() {
            editor?.remove();
            textarea.remove();
        }
    };
}
