export enum Priority {
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

export interface Task {
    id: number;
    title: string;
    description: string;
    dueDate: string;
    priority: Priority;
    priorityName: string;
    fullName: string;
    telephone: string;
    email: string;
    createdAt: string;
    updatedAt: string | null;
    isOverdue: boolean;
}

export interface CreateTaskDto {
    title: string;
    description: string;
    dueDate: string;
    priority: Priority;
    fullName: string;
    telephone: string;
    email: string;
}

export interface UpdateTaskDto {
    title: string;
    description: string;
    dueDate: string;
    priority: Priority;
    fullName: string;
    telephone: string;
    email: string;
}

export interface TaskFormData {
    title: string;
    description: string;
    dueDate: string;
    priority: Priority;
    fullName: string;
    telephone: string;
    email: string;
}

export interface ValidationErrors {
    title?: string;
    description?: string;
    dueDate?: string;
    priority?: string;
    fullName?: string;
    telephone?: string;
    email?: string;
}

export interface ApiError {
    message: string;
    errors?: Record<string, string[]>;
}
