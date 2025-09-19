import React from 'react';
import { Box, Typography, ToggleButton, ToggleButtonGroup } from '@mui/material';

const returnValueTypes = [
  { value: 'Boolean', label: 'Boolean' },
  { value: 'String', label: 'String' },
  { value: 'Number', label: 'Number' },
  { value: 'JSON', label: 'JSON' }
];

/**
 * Component for selecting flag return value type
 */
const ReturnTypeSelector = ({ 
  value, 
  onChange, 
  error,
  name = "returnValueType" 
}) => {
  const handleChange = (event, newValue) => {
    if (newValue !== null) {
      onChange(newValue);
    }
  };

  return (
    <Box sx={{ mb: 3 }}>
      <Typography variant="subtitle1" sx={{ mb: 2 }}>Flag type</Typography>
      <ToggleButtonGroup
        name={name}
        value={value}
        exclusive
        onChange={handleChange}
        sx={{
          '& .MuiToggleButton-root': {
            px: 2,
            py: 1,
            border: 1,
            borderColor: error ? 'error.main' : 'divider',
            borderRadius: 2,
            mx: 0.5,
            textTransform: 'none',
            fontWeight: 500,
            fontSize: '0.875rem',
            '&.Mui-selected': {
              backgroundColor: 'primary.main',
              color: 'primary.contrastText',
              '&:hover': {
                backgroundColor: 'primary.dark',
              }
            }
          }
        }}
      >
        {returnValueTypes.map((type) => (
          <ToggleButton key={type.value} value={type.value}>
            {type.label}
          </ToggleButton>
        ))}
      </ToggleButtonGroup>
      {error && (
        <Typography variant="caption" color="error" sx={{ mt: 1, display: 'block' }}>
          {error}
        </Typography>
      )}
    </Box>
  );
};

export default ReturnTypeSelector;
