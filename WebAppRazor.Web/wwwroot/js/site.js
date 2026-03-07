// SignalR Connection
if (window.userId) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveNotification", (title, message, type) => {
        showNotification(title, message, type);
    });

    connection.on("UpdateUnreadCount", (count) => {
        updateNotificationBadge(count);
    });

    connection.start()
        .then(() => {
            console.log("SignalR Connected");
            connection.invoke("JoinUserGroup", window.userId);
        })
        .catch(err => console.error(err.toString()));
}

function showNotification(title, message, type) {
    // Create notification element
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${getBootstrapColor(type)} border-0`;
    toast.role = 'alert';
    toast.ariaLive = 'assertive';
    toast.ariaAtomic = 'true';
    
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <strong>${title}</strong><br/>
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
    `;

    const container = getToastContainer();
    container.appendChild(toast);
    
    const bsToast = new bootstrap.Toast(toast, { delay: 5000 });
    bsToast.show();
}

function getBootstrapColor(type) {
    switch (type) {
        case 'MealReminder': return 'success';
        case 'ShoppingReminder': return 'primary';
        case 'Promotion': return 'warning';
        case 'Error': return 'danger';
        default: return 'info';
    }
}

function getToastContainer() {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(container);
    }
    return container;
}

function updateNotificationBadge(count) {
    const badges = document.querySelectorAll('.notification-badge');
    badges.forEach(badge => {
        if (count > 0) {
            badge.innerText = count;
            badge.classList.remove('d-none');
        } else {
            badge.classList.add('d-none');
        }
    });
}
