import { ValidationErrors, TaskFormData } from '../types/task.types';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const PHONE_REGEX = /^[\d\s\-+()]{7,20}$/;

export const validateTaskForm = (data: TaskFormData): ValidationErrors => {
    const errors: ValidationErrors = {};

    if (!data.title || data.title.trim() === '') {
        errors.title = 'Title is required';
    } else if (data.title.length < 1 || data.title.length > 100) {
        errors.title = 'Title must be between 1 and 100 characters';
    }

    if (!data.description || data.description.trim() === '') {
        errors.description = 'Description is required';
    } else if (data.description.length > 500) {
        errors.description = 'Description cannot exceed 500 characters';
    }

    if (!data.dueDate) {
        errors.dueDate = 'Due date is required';
    }

    if (data.priority === undefined || data.priority === null) {
        errors.priority = 'Priority is required';
    }

    if (!data.fullName || data.fullName.trim() === '') {
        errors.fullName = 'Full name is required';
    } else if (data.fullName.length < 2 || data.fullName.length > 100) {
        errors.fullName = 'Full name must be between 2 and 100 characters';
    }

    if (!data.telephone || data.telephone.trim() === '') {
        errors.telephone = 'Telephone is required';
    } else if (!PHONE_REGEX.test(data.telephone)) {
        errors.telephone = 'Invalid phone number format';
    } else if (data.telephone.length > 20) {
        errors.telephone = 'Telephone cannot exceed 20 characters';
    }

    if (!data.email || data.email.trim() === '') {
        errors.email = 'Email is required';
    } else if (!EMAIL_REGEX.test(data.email)) {
        errors.email = 'Invalid email address format';
    } else if (data.email.length > 100) {
        errors.email = 'Email cannot exceed 100 characters';
    }

    return errors;
};

export const hasValidationErrors = (errors: ValidationErrors): boolean => {
    return Object.keys(errors).length > 0;
};
