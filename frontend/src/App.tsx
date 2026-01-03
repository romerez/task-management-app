import React, { useEffect, useState } from 'react';
import { Provider } from 'react-redux';
import {
    ThemeProvider,
    createTheme,
    CssBaseline,
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
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { store } from './store/store';
import { useAppDispatch, useAppSelector } from './store/hooks';
import {
    fetchAllTasks,
    fetchOverdueTasks,
    createTask,
    updateTask,
    deleteTask,
    setSelectedTask,
    setFilter,
    clearError,
} from './store/taskSlice';
import { TaskForm } from './components/TaskForm';
import { TaskList } from './components/TaskList';
import { Task, TaskFormData, CreateTaskDto, UpdateTaskDto } from './types/task.types';

const theme = createTheme({
    palette: {
        primary: {
            main: '#1976d2',
        },
        secondary: {
            main: '#dc004e',
        },
        background: {
            default: '#f5f5f5',
        },
    },
    typography: {
        h4: {
            fontWeight: 600,
        },
    },
});

// Main App Content (uses Redux hooks)
const AppContent: React.FC = () => {
    const dispatch = useAppDispatch();
    const { tasks, selectedTask, isLoading, error, filter } = useAppSelector(
        (state) => state.tasks
    );
    const [isFormOpen, setIsFormOpen] = useState(false);

    // Fetch tasks on mount and when filter changes
    useEffect(() => {
        if (filter === 'all') {
            dispatch(fetchAllTasks());
        } else {
            dispatch(fetchOverdueTasks());
        }
    }, [dispatch, filter]);

    const handleCreateClick = () => {
        dispatch(setSelectedTask(null));
        setIsFormOpen(true);
    };

    const handleEditClick = (task: Task) => {
        dispatch(setSelectedTask(task));
        setIsFormOpen(true);
    };

    const handleFormCancel = () => {
        setIsFormOpen(false);
        dispatch(setSelectedTask(null));
    };

    const handleFormSubmit = async (data: TaskFormData) => {
        try {
            if (selectedTask) {
                const updateData: UpdateTaskDto = {
                    title: data.title,
                    description: data.description,
                    dueDate: new Date(data.dueDate).toISOString(),
                    priority: data.priority,
                    fullName: data.fullName,
                    telephone: data.telephone,
                    email: data.email,
                };
                await dispatch(updateTask({ id: selectedTask.id, task: updateData })).unwrap();
            } else {
                const createData: CreateTaskDto = {
                    title: data.title,
                    description: data.description,
                    dueDate: new Date(data.dueDate).toISOString(),
                    priority: data.priority,
                    fullName: data.fullName,
                    telephone: data.telephone,
                    email: data.email,
                };
                await dispatch(createTask(createData)).unwrap();
            }
            setIsFormOpen(false);
        } catch (err) {
            console.error('Failed to save task:', err);
        }
    };

    const handleDeleteClick = async (id: number) => {
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
        if (newFilter !== null) {
            dispatch(setFilter(newFilter));
        }
    };

    const handleCloseError = () => {
        dispatch(clearError());
    };

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <CssBaseline />

            {/* Header */}
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

            {/* Main Content */}
            <Container maxWidth="lg" sx={{ flex: 1, py: 4 }}>
                {/* Toolbar */}
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
                    <ToggleButtonGroup
                        value={filter}
                        exclusive
                        onChange={handleFilterChange}
                        size="small"
                    >
                        <ToggleButton value="all">All Tasks</ToggleButton>
                        <ToggleButton value="overdue">Overdue</ToggleButton>
                    </ToggleButtonGroup>

                    <Button
                        variant="contained"
                        startIcon={<AddIcon />}
                        onClick={handleCreateClick}
                    >
                        New Task
                    </Button>
                </Box>

                {/* Task Count */}
                <Box sx={{ mb: 2 }}>
                    <Chip
                        label={`Showing ${tasks.length} task${tasks.length !== 1 ? 's' : ''}${filter === 'overdue' ? ' (overdue)' : ''}`}
                        variant="outlined"
                        size="small"
                    />
                </Box>

                {/* Task List */}
                <TaskList
                    tasks={tasks}
                    isLoading={isLoading}
                    error={null}
                    onEdit={handleEditClick}
                    onDelete={handleDeleteClick}
                />
            </Container>

            {/* Task Form Dialog */}
            {isFormOpen && (
                <TaskForm
                    task={selectedTask}
                    onSubmit={handleFormSubmit}
                    onCancel={handleFormCancel}
                    isLoading={isLoading}
                />
            )}

            {/* Error Snackbar */}
            <Snackbar
                open={!!error}
                autoHideDuration={5000}
                onClose={handleCloseError}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert onClose={handleCloseError} severity="error" sx={{ width: '100%' }}>
                    {error}
                </Alert>
            </Snackbar>

            {/* Footer */}
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
        </Box>
    );
};

// App wrapper with Redux Provider and MUI Theme
const App: React.FC = () => {
    return (
        <ThemeProvider theme={theme}>
            <Provider store={store}>
                <AppContent />
            </Provider>
        </ThemeProvider>
    );
};

export default App;