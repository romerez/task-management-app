import { Priority } from '../types/task.types';

export const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
};

export const formatDateTime = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
    });
};

export const formatDateForInput = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toISOString().split('T')[0];
};

export const getPriorityLabel = (priority: Priority): string => {
    const labels: Record<Priority, string> = {
        [Priority.Low]: 'Low',
        [Priority.Medium]: 'Medium',
        [Priority.High]: 'High',
        [Priority.Critical]: 'Critical',
    };
    return labels[priority];
};

export const getPriorityColor = (priority: Priority): string => {
    const colors: Record<Priority, string> = {
        [Priority.Low]: '#4caf50',
        [Priority.Medium]: '#ff9800',
        [Priority.High]: '#f44336',
        [Priority.Critical]: '#9c27b0',
    };
    return colors[priority];
};
