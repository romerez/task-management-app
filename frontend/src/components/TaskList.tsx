import React from 'react';
import {
    Box,
    Typography,
    CircularProgress,
    Alert,
    Paper,
} from '@mui/material';
import { Assignment as AssignmentIcon } from '@mui/icons-material';
import { Task } from '../types/task.types';
import { TaskCard } from './TaskCard';

interface TaskListProps {
    tasks: Task[];
    isLoading: boolean;
    error: string | null;
    onEdit: (task: Task) => void;
    onDelete: (id: number) => void;
}

export const TaskList: React.FC<TaskListProps> = ({
    tasks,
    isLoading,
    error,
    onEdit,
    onDelete,
}) => {
    if (isLoading) {
        return (
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    py: 8,
                }}
            >
                <CircularProgress size={48} />
                <Typography variant="body1" sx={{ mt: 2 }} color="text.secondary">
                    Loading tasks...
                </Typography>
            </Box>
        );
    }

    if (error) {
        return (
            <Alert severity="error" sx={{ my: 2 }}>
                Error: {error}
            </Alert>
        );
    }

    if (tasks.length === 0) {
        return (
            <Paper
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    py: 8,
                    px: 4,
                    textAlign: 'center',
                }}
                elevation={0}
            >
                <AssignmentIcon sx={{ fontSize: 64, color: 'grey.400', mb: 2 }} />
                <Typography variant="h6" color="text.secondary" gutterBottom>
                    No tasks found
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    Create a new task to get started!
                </Typography>
            </Paper>
        );
    }

    return (
        <Box
            sx={{
                display: 'grid',
                gridTemplateColumns: {
                    xs: '1fr',
                    sm: 'repeat(2, 1fr)',
                    md: 'repeat(3, 1fr)',
                },
                gap: 3,
            }}
        >
            {tasks.map((task) => (
                <TaskCard
                    key={task.id}
                    task={task}
                    onEdit={onEdit}
                    onDelete={onDelete}
                />
            ))}
        </Box>
    );
};

export default TaskList;