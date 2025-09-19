import React from 'react';
import {
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormHelperText
} from '@mui/material';
import { getDefaultValueForTypeAndState } from '../../utils/flagUtils';
import { RETURN_VALUE_TYPES } from '../../constants/flagConstants';

const ReturnValueInput = ({
  returnValueType,
  value,
  onChange,
  name,
  label,
  isOffValue = false,
  error,
  helperText,
  size = "small",
  fullWidth = true
}) => {
  const defaultValue = getDefaultValueForTypeAndState(returnValueType, isOffValue);


  switch (returnValueType) {
    case RETURN_VALUE_TYPES.BOOLEAN:
      return (
          <FormControl fullWidth={fullWidth} size={size} error={!!error}>
            <InputLabel>{label}</InputLabel>
            <Select
                variant="outlined"
                name={name}
                value={value || defaultValue}
                label={label}
                onChange={onChange}
            >
              <MenuItem value="true">true</MenuItem>
              <MenuItem value="false">false</MenuItem>
            </Select>
            {error && <FormHelperText>{error}</FormHelperText>}
          </FormControl>
      );

    case RETURN_VALUE_TYPES.NUMBER:
      return (
          <TextField
              fullWidth={fullWidth}
              type="number"
              name={name}
              label={label}
              value={value || ''}
              onChange={onChange}
              error={!!error}
              helperText={helperText || error}
              size={size}
          />
      );

    case RETURN_VALUE_TYPES.JSON:
      return (
          <TextField
              fullWidth={fullWidth}
              type="text"
              name={name}
              label={label}
              value={value || ''}
              onChange={onChange}
              error={!!error}
              helperText={helperText || error}
              size={size}
              multiline
              rows={3}
          />
      );

    case RETURN_VALUE_TYPES.STRING:
    default:
      return (
          <TextField
              fullWidth={fullWidth}
              type="text"
              name={name}
              label={label}
              value={value || ''}
              onChange={onChange}
              error={!!error}
              helperText={helperText || error}
              size={size}
          />
      );
  }
};

export default ReturnValueInput;
