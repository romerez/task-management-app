import React, { useEffect, useState } from 'react';
import {
    AppBar,
    Toolbar,
    Typography,
    Container,
    Box,
    Button,
    ToggleButtonGroup,
    ToggleButton,
    Alert,
    Snackbar,
    Chip,
    CssBaseline,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import {
    fetchAllTasks,
    fetchOverdueTasks,
    createTask,
    updateTask,
    deleteTask,
    setSelectedTask,
    setFilter,
    clearError,
} from '../store/taskSlice';
import { TaskForm } from './TaskForm';
import { TaskList } from './TaskList';
import { Task, TaskFormData, CreateTaskDto, UpdateTaskDto } from '../types/task.types';

export const AppContent: React.FC = () => {
    const dispatch = useAppDispatch();
    const { tasks, selectedTask, isLoading, error, filter } = useAppSelector(
        (state) => state.tasks
    );
    const [isFormOpen, setIsFormOpen] = useState(false);

    useEffect(() => {
        const fetchTasks = filter === 'all' ? fetchAllTasks : fetchOverdueTasks;
        dispatch(fetchTasks());
    }, [dispatch, filter]);

    const openCreateForm = () => {
        dispatch(setSelectedTask(null));
        setIsFormOpen(true);
    };

    const openEditForm = (task: Task) => {
        dispatch(setSelectedTask(task));
        setIsFormOpen(true);
    };

    const closeForm = () => {
        setIsFormOpen(false);
        dispatch(setSelectedTask(null));
    };

    const buildTaskDto = (data: TaskFormData): CreateTaskDto | UpdateTaskDto => ({
        title: data.title,
        description: data.description,
        dueDate: new Date(data.dueDate).toISOString(),
        priority: data.priority,
        fullName: data.fullName,
        telephone: data.telephone,
        email: data.email,
    });

    const handleFormSubmit = async (data: TaskFormData) => {
        try {
            const taskDto = buildTaskDto(data);

            if (selectedTask) {
                await dispatch(updateTask({ id: selectedTask.id, task: taskDto })).unwrap();
            } else {
                await dispatch(createTask(taskDto)).unwrap();
            }
            closeForm();
        } catch (err) {
            console.error('Failed to save task:', err);
        }
    };

    const handleDelete = async (id: number) => {
        try {
            await dispatch(deleteTask(id)).unwrap();
        } catch (err) {
            console.error('Failed to delete task:', err);
        }
    };

    const handleFilterChange = (
        _event: React.MouseEvent<HTMLElement>,
        newFilter: 'all' | 'overdue' | null
    ) => {
        if (newFilter) {
            dispatch(setFilter(newFilter));
        }
    };

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <CssBaseline />

            <Header />

            <Container maxWidth="lg" sx={{ flex: 1, py: 4 }}>
                <TaskToolbar
                    filter={filter}
                    onFilterChange={handleFilterChange}
                    onCreateClick={openCreateForm}
                />

                <TaskCountChip count={tasks.length} filter={filter} />

                <TaskList
                    tasks={tasks}
                    isLoading={isLoading}
                    error={null}
                    onEdit={openEditForm}
                    onDelete={handleDelete}
                />
            </Container>

            {isFormOpen && (
                <TaskForm
                    task={selectedTask}
                    onSubmit={handleFormSubmit}
                    onCancel={closeForm}
                    isLoading={isLoading}
                />
            )}

            <ErrorSnackbar error={error} onClose={() => dispatch(clearError())} />

            <Footer />
        </Box>
    );
};

const Header: React.FC = () => (
    <AppBar position="static" elevation={2}>
        <Toolbar>
            <Typography variant="h5" component="h1" sx={{ flexGrow: 1, fontWeight: 600 }}>
                Task Manager
            </Typography>
            <Typography variant="body2" sx={{ opacity: 0.8 }}>
                Manage your tasks efficiently
            </Typography>
        </Toolbar>
    </AppBar>
);

interface TaskToolbarProps {
    filter: 'all' | 'overdue';
    onFilterChange: (event: React.MouseEvent<HTMLElement>, value: 'all' | 'overdue' | null) => void;
    onCreateClick: () => void;
}

const TaskToolbar: React.FC<TaskToolbarProps> = ({ filter, onFilterChange, onCreateClick }) => (
    <Box
        sx={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            mb: 3,
            flexWrap: 'wrap',
            gap: 2,
        }}
    >
        <ToggleButtonGroup value={filter} exclusive onChange={onFilterChange} size="small">
            <ToggleButton value="all">All Tasks</ToggleButton>
            <ToggleButton value="overdue">Overdue</ToggleButton>
        </ToggleButtonGroup>

        <Button variant="contained" startIcon={<AddIcon />} onClick={onCreateClick}>
            New Task
        </Button>
    </Box>
);

interface TaskCountChipProps {
    count: number;
    filter: 'all' | 'overdue';
}

const TaskCountChip: React.FC<TaskCountChipProps> = ({ count, filter }) => (
    <Box sx={{ mb: 2 }}>
        <Chip
            label={`Showing ${count} task${count !== 1 ? 's' : ''}${filter === 'overdue' ? ' (overdue)' : ''}`}
            variant="outlined"
            size="small"
        />
    </Box>
);

interface ErrorSnackbarProps {
    error: string | null;
    onClose: () => void;
}

const ErrorSnackbar: React.FC<ErrorSnackbarProps> = ({ error, onClose }) => (
    <Snackbar
        open={!!error}
        autoHideDuration={5000}
        onClose={onClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
    >
        <Alert onClose={onClose} severity="error" sx={{ width: '100%' }}>
            {error}
        </Alert>
    </Snackbar>
);

const Footer: React.FC = () => (
    <Box
        component="footer"
        sx={{
            py: 2,
            px: 2,
            mt: 'auto',
            backgroundColor: 'grey.100',
            textAlign: 'center',
        }}
    >
        <Typography variant="body2" color="text.secondary">
            © 2026 Task Manager Application
        </Typography>
    </Box>
);
