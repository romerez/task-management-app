import { ValidationErrors, TaskFormData } from '../types/task.types';

const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const PHONE_REGEX = /^[\d\s\-+()]{7,20}$/;

type Validator = (value: unknown, data: TaskFormData) => string | undefined;

const required = (message: string): Validator =>
    (value) => (!value || (typeof value === 'string' && !value.trim())) ? message : undefined;

const minLength = (min: number, message: string): Validator =>
    (value) => (typeof value === 'string' && value.length < min) ? message : undefined;

const maxLength = (max: number, message: string): Validator =>
    (value) => (typeof value === 'string' && value.length > max) ? message : undefined;

const pattern = (regex: RegExp, message: string): Validator =>
    (value) => (typeof value === 'string' && !regex.test(value)) ? message : undefined;

const combineValidators = (...validators: Validator[]): Validator =>
    (value, data) => {
        for (const validator of validators) {
            const error = validator(value, data);
            if (error) return error;
        }
        return undefined;
    };

const fieldValidators: Record<keyof TaskFormData, Validator> = {
    title: combineValidators(
        required('Title is required'),
        minLength(1, 'Title must be between 1 and 100 characters'),
        maxLength(100, 'Title must be between 1 and 100 characters')
    ),
    description: combineValidators(
        required('Description is required'),
        maxLength(500, 'Description cannot exceed 500 characters')
    ),
    dueDate: required('Due date is required'),
    priority: (value) => (value === undefined || value === null) ? 'Priority is required' : undefined,
    fullName: combineValidators(
        required('Full name is required'),
        minLength(2, 'Full name must be between 2 and 100 characters'),
        maxLength(100, 'Full name must be between 2 and 100 characters')
    ),
    telephone: combineValidators(
        required('Telephone is required'),
        pattern(PHONE_REGEX, 'Invalid phone number format'),
        maxLength(20, 'Telephone cannot exceed 20 characters')
    ),
    email: combineValidators(
        required('Email is required'),
        pattern(EMAIL_REGEX, 'Invalid email address format'),
        maxLength(100, 'Email cannot exceed 100 characters')
    ),
};

export const validateTaskForm = (data: TaskFormData): ValidationErrors => {
    const errors: ValidationErrors = {};

    for (const [field, validator] of Object.entries(fieldValidators)) {
        const key = field as keyof TaskFormData;
        const error = validator(data[key], data);
        if (error) {
            errors[key] = error;
        }
    }

    return errors;
};

export const hasValidationErrors = (errors: ValidationErrors): boolean =>
    Object.keys(errors).length > 0;
