import React, { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Box,
    Typography,
    IconButton,
    CircularProgress,
    Divider,
} from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';
import { Task, TaskFormData, ValidationErrors, Priority } from '../types/task.types';
import { validateTaskForm, hasValidationErrors, formatDateForInput } from '../utils';

interface TaskFormProps {
    task?: Task | null;
    onSubmit: (data: TaskFormData) => void;
    onCancel: () => void;
    isLoading: boolean;
}

const initialFormData: TaskFormData = {
    title: '',
    description: '',
    dueDate: '',
    priority: Priority.Medium,
    fullName: '',
    telephone: '',
    email: '',
};

export const TaskForm: React.FC<TaskFormProps> = ({
    task,
    onSubmit,
    onCancel,
    isLoading,
}) => {
    const [formData, setFormData] = useState<TaskFormData>(initialFormData);
    const [errors, setErrors] = useState<ValidationErrors>({});
    const [touched, setTouched] = useState<Record<string, boolean>>({});

    useEffect(() => {
        if (task) {
            setFormData({
                title: task.title,
                description: task.description,
                dueDate: formatDateForInput(task.dueDate),
                priority: task.priority,
                fullName: task.fullName,
                telephone: task.telephone,
                email: task.email,
            });
        } else {
            setFormData(initialFormData);
        }
        setErrors({});
        setTouched({});
    }, [task]);

    const handleChange = (name: string, value: string | number) => {
        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));

        if (touched[name]) {
            const newErrors = validateTaskForm({
                ...formData,
                [name]: value,
            });
            setErrors((prev) => ({
                ...prev,
                [name]: newErrors[name as keyof ValidationErrors],
            }));
        }
    };

    const handleBlur = (name: string) => {
        setTouched((prev) => ({ ...prev, [name]: true }));
        const newErrors = validateTaskForm(formData);
        setErrors((prev) => ({
            ...prev,
            [name]: newErrors[name as keyof ValidationErrors],
        }));
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        const validationErrors = validateTaskForm(formData);
        setErrors(validationErrors);

        const allTouched: Record<string, boolean> = {};
        Object.keys(formData).forEach((key) => {
            allTouched[key] = true;
        });
        setTouched(allTouched);

        if (!hasValidationErrors(validationErrors)) {
            onSubmit(formData);
        }
    };

    const isEditing = !!task;

    return (
        <Dialog
            open={true}
            onClose={onCancel}
            maxWidth="sm"
            fullWidth
            PaperProps={{ sx: { borderRadius: 2 } }}
        >
            <DialogTitle sx={{ m: 0, p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Typography variant="h6" component="span">
                    {isEditing ? 'Edit Task' : 'Create New Task'}
                </Typography>
                <IconButton
                    aria-label="close"
                    onClick={onCancel}
                    sx={{ color: 'grey.500' }}
                >
                    <CloseIcon />
                </IconButton>
            </DialogTitle>

            <form onSubmit={handleSubmit}>
                <DialogContent dividers>
                    <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
                        Task Details
                    </Typography>

                    <TextField
                        fullWidth
                        label="Title"
                        name="title"
                        value={formData.title}
                        onChange={(e) => handleChange('title', e.target.value)}
                        onBlur={() => handleBlur('title')}
                        error={!!(errors.title && touched.title)}
                        helperText={touched.title && errors.title}
                        required
                        margin="normal"
                    />

                    <TextField
                        fullWidth
                        label="Description"
                        name="description"
                        value={formData.description}
                        onChange={(e) => handleChange('description', e.target.value)}
                        onBlur={() => handleBlur('description')}
                        error={!!(errors.description && touched.description)}
                        helperText={
                            (touched.description && errors.description) ||
                            `${formData.description.length}/500`
                        }
                        required
                        multiline
                        rows={4}
                        margin="normal"
                        inputProps={{ maxLength: 500 }}
                    />

                    <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
                        <TextField
                            fullWidth
                            label="Due Date"
                            name="dueDate"
                            type="date"
                            value={formData.dueDate}
                            onChange={(e) => handleChange('dueDate', e.target.value)}
                            onBlur={() => handleBlur('dueDate')}
                            error={!!(errors.dueDate && touched.dueDate)}
                            helperText={touched.dueDate && errors.dueDate}
                            required
                        />
                        <FormControl fullWidth error={!!(errors.priority && touched.priority)}>
                            <InputLabel>Priority *</InputLabel>
                            <Select
                                value={formData.priority}
                                label="Priority *"
                                onChange={(e) => handleChange('priority', e.target.value as number)}
                                onBlur={() => handleBlur('priority')}
                            >
                                <MenuItem value={Priority.Low}>Low</MenuItem>
                                <MenuItem value={Priority.Medium}>Medium</MenuItem>
                                <MenuItem value={Priority.High}>High</MenuItem>
                                <MenuItem value={Priority.Critical}>Critical</MenuItem>
                            </Select>
                        </FormControl>
                    </Box>

                    <Divider sx={{ my: 3 }} />
                    <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>
                        User Details
                    </Typography>

                    <TextField
                        fullWidth
                        label="Full Name"
                        name="fullName"
                        value={formData.fullName}
                        onChange={(e) => handleChange('fullName', e.target.value)}
                        onBlur={() => handleBlur('fullName')}
                        error={!!(errors.fullName && touched.fullName)}
                        helperText={touched.fullName && errors.fullName}
                        required
                        margin="normal"
                        inputProps={{ maxLength: 100 }}
                    />

                    <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
                        <TextField
                            fullWidth
                            label="Telephone"
                            name="telephone"
                            type="tel"
                            value={formData.telephone}
                            onChange={(e) => handleChange('telephone', e.target.value)}
                            onBlur={() => handleBlur('telephone')}
                            error={!!(errors.telephone && touched.telephone)}
                            helperText={touched.telephone && errors.telephone}
                            required
                            inputProps={{ maxLength: 20 }}
                        />
                        <TextField
                            fullWidth
                            label="Email"
                            name="email"
                            type="email"
                            value={formData.email}
                            onChange={(e) => handleChange('email', e.target.value)}
                            onBlur={() => handleBlur('email')}
                            error={!!(errors.email && touched.email)}
                            helperText={touched.email && errors.email}
                            required
                            inputProps={{ maxLength: 100 }}
                        />
                    </Box>
                </DialogContent>

                <DialogActions sx={{ px: 3, py: 2 }}>
                    <Button onClick={onCancel} disabled={isLoading}>
                        Cancel
                    </Button>
                    <Button
                        type="submit"
                        variant="contained"
                        disabled={isLoading}
                        startIcon={isLoading ? <CircularProgress size={20} /> : null}
                    >
                        {isLoading ? 'Saving...' : isEditing ? 'Update Task' : 'Create Task'}
                    </Button>
                </DialogActions>
            </form>
        </Dialog>
    );
};

export default TaskForm;