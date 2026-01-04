import { apiClient } from './apiClient';
import { Task, CreateTaskDto, UpdateTaskDto } from '../types/task.types';

const TASKS_ENDPOINT = '/tasks';

export const taskService = {
    getAllTasks: (): Promise<Task[]> =>
        apiClient.get<Task[]>(TASKS_ENDPOINT),

    getTaskById: (id: number): Promise<Task> =>
        apiClient.get<Task>(`${TASKS_ENDPOINT}/${id}`),

    getOverdueTasks: (): Promise<Task[]> =>
        apiClient.get<Task[]>(`${TASKS_ENDPOINT}/overdue`),

    createTask: (task: CreateTaskDto): Promise<Task> =>
        apiClient.post<Task>(TASKS_ENDPOINT, task),

    updateTask: (id: number, task: UpdateTaskDto): Promise<Task> =>
        apiClient.put<Task>(`${TASKS_ENDPOINT}/${id}`, task),

    deleteTask: (id: number): Promise<void> =>
        apiClient.delete<void>(`${TASKS_ENDPOINT}/${id}`),
};

