import axios, { AxiosError } from 'axios';
import { Task, CreateTaskDto, UpdateTaskDto, ApiError } from '../types/task.types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api';

const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

const handleApiError = (error: unknown): never => {
    if (axios.isAxiosError(error)) {
        const axiosError = error as AxiosError<ApiError>;
        if (axiosError.response?.data?.message) {
            throw new Error(axiosError.response.data.message);
        }
        if (axiosError.response?.status === 404) {
            throw new Error('Task not found');
        }
        if (axiosError.response?.status === 400) {
            throw new Error('Invalid request data');
        }
    }
    throw new Error('An unexpected error occurred');
};

export const taskService = {
    getAllTasks: async (): Promise<Task[]> => {
        try {
            const response = await apiClient.get<Task[]>('/tasks');
            return response.data;
        } catch (error) {
            throw handleApiError(error);
        }
    },

    getTaskById: async (id: number): Promise<Task> => {
        try {
            const response = await apiClient.get<Task>(`/tasks/${id}`);
            return response.data;
        } catch (error) {
            throw handleApiError(error);
        }
    },

    getOverdueTasks: async (): Promise<Task[]> => {
        try {
            const response = await apiClient.get<Task[]>('/tasks/overdue');
            return response.data;
        } catch (error) {
            throw handleApiError(error);
        }
    },

    createTask: async (task: CreateTaskDto): Promise<Task> => {
        try {
            const response = await apiClient.post<Task>('/tasks', task);
            return response.data;
        } catch (error) {
            throw handleApiError(error);
        }
    },

    updateTask: async (id: number, task: UpdateTaskDto): Promise<Task> => {
        try {
            const response = await apiClient.put<Task>(`/tasks/${id}`, task);
            return response.data;
        } catch (error) {
            throw handleApiError(error);
        }
    },

    deleteTask: async (id: number): Promise<void> => {
        try {
            await apiClient.delete(`/tasks/${id}`);
        } catch (error) {
            throw handleApiError(error);
        }
    },
};

export default taskService;
