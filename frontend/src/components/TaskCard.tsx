import React, { useState } from 'react';
import {
    Card,
    CardContent,
    CardActions,
    Typography,
    Chip,
    Box,
    Button,
    Divider,
    Stack,
} from '@mui/material';
import {
    Edit as EditIcon,
    Delete as DeleteIcon,
    Person as PersonIcon,
    Phone as PhoneIcon,
    Email as EmailIcon,
    Event as EventIcon,
} from '@mui/icons-material';
import { Task, Priority } from '../types/task.types';
import { formatDate, formatDateTime, getPriorityLabel } from '../utils';
import { ConfirmDialog } from './ConfirmDialog';

interface TaskCardProps {
    task: Task;
    onEdit: (task: Task) => void;
    onDelete: (id: number) => void;
}

const getPriorityColor = (priority: Priority): 'success' | 'warning' | 'error' | 'secondary' => {
    switch (priority) {
        case Priority.Low:
            return 'success';
        case Priority.Medium:
            return 'warning';
        case Priority.High:
            return 'error';
        case Priority.Critical:
            return 'secondary';
        default:
            return 'warning';
    }
};

export const TaskCard: React.FC<TaskCardProps> = ({ task, onEdit, onDelete }) => {
    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

    const handleDeleteClick = () => {
        setIsDeleteDialogOpen(true);
    };

    const handleDeleteConfirm = () => {
        setIsDeleteDialogOpen(false);
        onDelete(task.id);
    };

    const handleDeleteCancel = () => {
        setIsDeleteDialogOpen(false);
    };

    return (
        <>
            <Card
                sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    borderLeft: task.isOverdue ? '4px solid' : 'none',
                    borderColor: task.isOverdue ? 'error.main' : 'transparent',
                }}
                elevation={2}
            >
                <CardContent sx={{ flex: 1 }}>
                    {/* Header */}
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                        <Typography variant="h6" component="h3" sx={{ fontWeight: 600, flex: 1, mr: 1 }}>
                            {task.title}
                        </Typography>
                        <Chip
                            label={getPriorityLabel(task.priority)}
                            color={getPriorityColor(task.priority)}
                            size="small"
                        />
                    </Box>

                    {/* Description */}
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                        {task.description}
                    </Typography>

                    {/* Due Date */}
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, gap: 1 }}>
                        <EventIcon fontSize="small" color="action" />
                        <Typography variant="body2" color={task.isOverdue ? 'error.main' : 'text.secondary'}>
                            Due: {formatDate(task.dueDate)}
                        </Typography>
                        {task.isOverdue && (
                            <Chip label="OVERDUE" color="error" size="small" variant="outlined" />
                        )}
                    </Box>

                    <Divider sx={{ my: 2 }} />

                    {/* Assigned To Section */}
                    <Typography variant="subtitle2" sx={{ mb: 1, fontWeight: 600 }}>
                        Assigned To
                    </Typography>
                    <Stack spacing={1}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <PersonIcon fontSize="small" color="action" />
                            <Typography variant="body2">{task.fullName}</Typography>
                        </Box>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <PhoneIcon fontSize="small" color="action" />
                            <Typography variant="body2">{task.telephone}</Typography>
                        </Box>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <EmailIcon fontSize="small" color="action" />
                            <Typography variant="body2">{task.email}</Typography>
                        </Box>
                    </Stack>

                    {/* Meta info */}
                    <Box sx={{ mt: 2, pt: 2, borderTop: '1px solid', borderColor: 'divider' }}>
                        <Typography variant="caption" color="text.secondary" display="block">
                            Created: {formatDateTime(task.createdAt)}
                        </Typography>
                        {task.updatedAt && (
                            <Typography variant="caption" color="text.secondary" display="block">
                                Updated: {formatDateTime(task.updatedAt)}
                            </Typography>
                        )}
                    </Box>
                </CardContent>

                <CardActions sx={{ justifyContent: 'flex-end', px: 2, pb: 2 }}>
                    <Button
                        size="small"
                        startIcon={<EditIcon />}
                        onClick={() => onEdit(task)}
                    >
                        Edit
                    </Button>
                    <Button
                        size="small"
                        color="error"
                        startIcon={<DeleteIcon />}
                        onClick={handleDeleteClick}
                    >
                        Delete
                    </Button>
                </CardActions>
            </Card>

            <ConfirmDialog
                open={isDeleteDialogOpen}
                title="Delete Task"
                message="Are you sure you want to delete this task? This action cannot be undone."
                confirmText="Delete"
                cancelText="Cancel"
                onConfirm={handleDeleteConfirm}
                onCancel={handleDeleteCancel}
            />
        </>
    );
};

export default TaskCard;