import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7001/api';

const axiosInstance = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export interface ApiErrorResponse {
    message: string;
    errors?: Record<string, string[]>;
}

const handleError = (error: unknown): never => {
    if (axios.isAxiosError(error)) {
        const axiosError = error as AxiosError<ApiErrorResponse>;
        const message = axiosError.response?.data?.message;

        if (message) {
            throw new Error(message);
        }

        const status = axiosError.response?.status;
        const errorMessages: Record<number, string> = {
            400: 'Invalid request data',
            401: 'Unauthorized',
            403: 'Forbidden',
            404: 'Resource not found',
            500: 'Internal server error',
        };

        throw new Error(errorMessages[status || 0] || 'An unexpected error occurred');
    }

    throw new Error('An unexpected error occurred');
};

export const apiClient = {
    get: async <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
        try {
            const response: AxiosResponse<T> = await axiosInstance.get(url, config);
            return response.data;
        } catch (error) {
            throw handleError(error);
        }
    },

    post: async <T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> => {
        try {
            const response: AxiosResponse<T> = await axiosInstance.post(url, data, config);
            return response.data;
        } catch (error) {
            throw handleError(error);
        }
    },

    put: async <T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<T> => {
        try {
            const response: AxiosResponse<T> = await axiosInstance.put(url, data, config);
            return response.data;
        } catch (error) {
            throw handleError(error);
        }
    },

    delete: async <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
        try {
            const response: AxiosResponse<T> = await axiosInstance.delete(url, config);
            return response.data;
        } catch (error) {
            throw handleError(error);
        }
    },
};
