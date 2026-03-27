// Sweet Alert notifications
function showSuccess(message) {
    Swal.fire({
        icon: 'success',
        title: 'Thành công!',
        text: message,
        timer: 3000,
        showConfirmButton: false,
        toast: true,
        position: 'top-end'
    });
}

function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Lỗi!',
        text: message,
        timer: 3000,
        showConfirmButton: false,
        toast: true,
        position: 'top-end'
    });
}

function showWarning(message) {
    Swal.fire({
        icon: 'warning',
        title: 'Cảnh báo!',
        text: message,
        timer: 3000,
        showConfirmButton: false,
        toast: true,
        position: 'top-end'
    });
}

// Confirm delete
function confirmDelete(url, name) {
    Swal.fire({
        title: 'Xác nhận xóa',
        text: `Bạn có chắc muốn xóa "${name}" không?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}

// Add animation to elements on scroll
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('animate-fade-in');
            observer.unobserve(entry.target);
        }
    });
}, observerOptions);

document.querySelectorAll('[data-animate]').forEach(el => {
    observer.observe(el);
});

// Form validation with animation
document.querySelectorAll('.input-field').forEach(input => {
    input.addEventListener('focus', () => {
        input.parentElement.classList.add('transform', 'scale-105');
    });
    
    input.addEventListener('blur', () => {
        input.parentElement.classList.remove('transform', 'scale-105');
    });
});