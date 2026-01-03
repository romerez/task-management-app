import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { Task, CreateTaskDto, UpdateTaskDto } from '../types/task.types';
import { taskService } from '../services/taskService';

interface TaskState {
    tasks: Task[];
    selectedTask: Task | null;
    isLoading: boolean;
    error: string | null;
    filter: 'all' | 'overdue';
}

const initialState: TaskState = {
    tasks: [],
    selectedTask: null,
    isLoading: false,
    error: null,
    filter: 'all',
};

export const fetchAllTasks = createAsyncThunk(
    'tasks/fetchAll',
    async (_, { rejectWithValue }) => {
        try {
            const tasks = await taskService.getAllTasks();
            return tasks;
        } catch (error) {
            return rejectWithValue((error as Error).message);
        }
    }
);

export const fetchOverdueTasks = createAsyncThunk(
    'tasks/fetchOverdue',
    async (_, { rejectWithValue }) => {
        try {
            const tasks = await taskService.getOverdueTasks();
            return tasks;
        } catch (error) {
            return rejectWithValue((error as Error).message);
        }
    }
);

export const fetchTaskById = createAsyncThunk(
    'tasks/fetchById',
    async (id: number, { rejectWithValue }) => {
        try {
            const task = await taskService.getTaskById(id);
            return task;
        } catch (error) {
            return rejectWithValue((error as Error).message);
        }
    }
);

export const createTask = createAsyncThunk(
    'tasks/create',
    async (task: CreateTaskDto, { rejectWithValue }) => {
        try {
            const newTask = await taskService.createTask(task);
            return newTask;
        } catch (error) {
            return rejectWithValue((error as Error).message);
        }
    }
);

export const updateTask = createAsyncThunk(
    'tasks/update',
    async ({ id, task }: { id: number; task: UpdateTaskDto }, { rejectWithValue }) => {
        try {
            const updatedTask = await taskService.updateTask(id, task);
            return updatedTask;
        } catch (error) {
            return rejectWithValue((error as Error).message);
        }
    }
);

export const deleteTask = createAsyncThunk(
    'tasks/delete',
    async (id: number, { rejectWithValue }) => {
        try {
            await taskService.deleteTask(id);
            return id;
        } catch (error) {
            return rejectWithValue((error as Error).message);
        }
    }
);

const taskSlice = createSlice({
    name: 'tasks',
    initialState,
    reducers: {
        clearError: (state) => {
            state.error = null;
        },
        setSelectedTask: (state, action: PayloadAction<Task | null>) => {
            state.selectedTask = action.payload;
        },
        setFilter: (state, action: PayloadAction<'all' | 'overdue'>) => {
            state.filter = action.payload;
        },
    },
    extraReducers: (builder) => {
        builder.addCase(fetchAllTasks.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        });
        builder.addCase(fetchAllTasks.fulfilled, (state, action) => {
            state.isLoading = false;
            state.tasks = action.payload;
        });
        builder.addCase(fetchAllTasks.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload as string;
        });

        builder.addCase(fetchOverdueTasks.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        });
        builder.addCase(fetchOverdueTasks.fulfilled, (state, action) => {
            state.isLoading = false;
            state.tasks = action.payload;
        });
        builder.addCase(fetchOverdueTasks.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload as string;
        });

        builder.addCase(fetchTaskById.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        });
        builder.addCase(fetchTaskById.fulfilled, (state, action) => {
            state.isLoading = false;
            state.selectedTask = action.payload;
        });
        builder.addCase(fetchTaskById.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload as string;
        });

        builder.addCase(createTask.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        });
        builder.addCase(createTask.fulfilled, (state, action) => {
            state.isLoading = false;
            state.tasks.push(action.payload);
        });
        builder.addCase(createTask.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload as string;
        });

        builder.addCase(updateTask.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        });
        builder.addCase(updateTask.fulfilled, (state, action) => {
            state.isLoading = false;
            const index = state.tasks.findIndex((t) => t.id === action.payload.id);
            if (index !== -1) {
                state.tasks[index] = action.payload;
            }
            state.selectedTask = null;
        });
        builder.addCase(updateTask.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload as string;
        });

        builder.addCase(deleteTask.pending, (state) => {
            state.isLoading = true;
            state.error = null;
        });
        builder.addCase(deleteTask.fulfilled, (state, action) => {
            state.isLoading = false;
            state.tasks = state.tasks.filter((t) => t.id !== action.payload);
        });
        builder.addCase(deleteTask.rejected, (state, action) => {
            state.isLoading = false;
            state.error = action.payload as string;
        });
    },
});

export const { clearError, setSelectedTask, setFilter } = taskSlice.actions;
export default taskSlice.reducer;
