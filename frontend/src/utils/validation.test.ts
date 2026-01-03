import { validateTaskForm, hasValidationErrors } from './validation';
import { Priority, TaskFormData } from '../types/task.types';

describe('validateTaskForm', () => {
    const validFormData: TaskFormData = {
        title: 'Test Task',
        description: 'This is a test description',
        dueDate: '2026-01-15',
        priority: Priority.Medium,
        fullName: 'John Doe',
        telephone: '123-456-7890',
        email: 'john.doe@example.com',
    };

    describe('title validation', () => {
        it('should return error when title is empty', () => {
            const errors = validateTaskForm({ ...validFormData, title: '' });
            expect(errors.title).toBe('Title is required');
        });

        it('should return error when title exceeds 100 characters', () => {
            const errors = validateTaskForm({ ...validFormData, title: 'a'.repeat(101) });
            expect(errors.title).toBe('Title must be between 1 and 100 characters');
        });

        it('should not return error for valid title', () => {
            const errors = validateTaskForm(validFormData);
            expect(errors.title).toBeUndefined();
        });
    });

    describe('description validation', () => {
        it('should return error when description is empty', () => {
            const errors = validateTaskForm({ ...validFormData, description: '' });
            expect(errors.description).toBe('Description is required');
        });

        it('should return error when description exceeds 500 characters', () => {
            const errors = validateTaskForm({ ...validFormData, description: 'a'.repeat(501) });
            expect(errors.description).toBe('Description cannot exceed 500 characters');
        });

        it('should not return error for valid description', () => {
            const errors = validateTaskForm(validFormData);
            expect(errors.description).toBeUndefined();
        });
    });

    describe('dueDate validation', () => {
        it('should return error when dueDate is empty', () => {
            const errors = validateTaskForm({ ...validFormData, dueDate: '' });
            expect(errors.dueDate).toBe('Due date is required');
        });

        it('should not return error for valid dueDate', () => {
            const errors = validateTaskForm(validFormData);
            expect(errors.dueDate).toBeUndefined();
        });
    });

    describe('priority validation', () => {
        it('should return error when priority is undefined', () => {
            const errors = validateTaskForm({ ...validFormData, priority: undefined as any });
            expect(errors.priority).toBe('Priority is required');
        });

        it('should accept all priority levels', () => {
            [Priority.Low, Priority.Medium, Priority.High, Priority.Critical].forEach((priority) => {
                const errors = validateTaskForm({ ...validFormData, priority });
                expect(errors.priority).toBeUndefined();
            });
        });
    });

    describe('fullName validation', () => {
        it('should return error when fullName is empty', () => {
            const errors = validateTaskForm({ ...validFormData, fullName: '' });
            expect(errors.fullName).toBe('Full name is required');
        });

        it('should return error when fullName is less than 2 characters', () => {
            const errors = validateTaskForm({ ...validFormData, fullName: 'A' });
            expect(errors.fullName).toBe('Full name must be between 2 and 100 characters');
        });

        it('should return error when fullName exceeds 100 characters', () => {
            const errors = validateTaskForm({ ...validFormData, fullName: 'a'.repeat(101) });
            expect(errors.fullName).toBe('Full name must be between 2 and 100 characters');
        });

        it('should not return error for valid fullName', () => {
            const errors = validateTaskForm(validFormData);
            expect(errors.fullName).toBeUndefined();
        });
    });

    describe('telephone validation', () => {
        it('should return error when telephone is empty', () => {
            const errors = validateTaskForm({ ...validFormData, telephone: '' });
            expect(errors.telephone).toBe('Telephone is required');
        });

        it('should return error for invalid phone format', () => {
            const errors = validateTaskForm({ ...validFormData, telephone: 'abc' });
            expect(errors.telephone).toBe('Invalid phone number format');
        });

        it('should accept valid phone formats', () => {
            const validPhones = ['123-456-7890', '+1 234 567 8901', '(123) 456-7890', '1234567890'];
            validPhones.forEach((telephone) => {
                const errors = validateTaskForm({ ...validFormData, telephone });
                expect(errors.telephone).toBeUndefined();
            });
        });
    });

    describe('email validation', () => {
        it('should return error when email is empty', () => {
            const errors = validateTaskForm({ ...validFormData, email: '' });
            expect(errors.email).toBe('Email is required');
        });

        it('should return error for invalid email format', () => {
            const invalidEmails = ['invalid', 'invalid@', '@invalid.com'];
            invalidEmails.forEach((email) => {
                const errors = validateTaskForm({ ...validFormData, email });
                expect(errors.email).toBe('Invalid email address format');
            });
        });

        it('should return error when email exceeds 100 characters', () => {
            const errors = validateTaskForm({ ...validFormData, email: 'a'.repeat(92) + '@test.com' });
            expect(errors.email).toBe('Email cannot exceed 100 characters');
        });

        it('should not return error for valid email', () => {
            const errors = validateTaskForm(validFormData);
            expect(errors.email).toBeUndefined();
        });
    });

    describe('complete form validation', () => {
        it('should return no errors for valid form data', () => {
            const errors = validateTaskForm(validFormData);
            expect(hasValidationErrors(errors)).toBe(false);
        });

        it('should return multiple errors for invalid form data', () => {
            const invalidFormData: TaskFormData = {
                title: '',
                description: '',
                dueDate: '',
                priority: undefined as any,
                fullName: '',
                telephone: '',
                email: '',
            };
            const errors = validateTaskForm(invalidFormData);
            expect(hasValidationErrors(errors)).toBe(true);
            expect(Object.keys(errors).length).toBe(7);
        });
    });
});

describe('hasValidationErrors', () => {
    it('should return false for empty errors object', () => {
        expect(hasValidationErrors({})).toBe(false);
    });

    it('should return true when errors exist', () => {
        expect(hasValidationErrors({ title: 'Error' })).toBe(true);
    });
});
