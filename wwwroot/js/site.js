// Download file helper
window.downloadFile = function (dataUrl, filename) {
    const link = document.createElement('a');
    link.href = dataUrl;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// Show toast notification
window.showToast = function (message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast-notification toast-${type}`;
    
    // Add icon based on type
    const icons = {
        success: '✓',
        error: '✗',
        info: 'ℹ',
        warning: '⚠'
    };
    
    const icon = document.createElement('span');
    icon.style.marginRight = '0.5rem';
    icon.style.fontSize = '1.2rem';
    icon.textContent = icons[type] || icons.info;
    
    const text = document.createTextNode(message);
    
    toast.appendChild(icon);
    toast.appendChild(text);
    document.body.appendChild(toast);

    // Trigger animation
    setTimeout(() => toast.classList.add('show'), 10);

    // Remove after 3 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => {
            if (toast.parentNode) {
                document.body.removeChild(toast);
            }
        }, 300);
    }, 3000);
};

// Smooth scroll to element
window.scrollToElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        const offsetTop = element.offsetTop - 20; // 20px padding from top
        window.scrollTo({
            top: offsetTop,
            behavior: 'smooth'
        });
    }
};
