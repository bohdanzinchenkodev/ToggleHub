import React, { useState, useEffect } from 'react';
import {
  Box,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  IconButton,
  Grid,
  FormHelperText
} from '@mui/material';
import { Delete as DeleteIcon } from '@mui/icons-material';
import { RULE_FIELD_TYPES, OPERATOR_TYPES, VALID_OPERATORS } from '../../constants/flagConstants';

/**
 * Component for managing individual rule conditions
 */
const ConditionCard = ({
                         condition,
                         index,
                         onUpdate,
                         onRemove,
                         getError
                       }) => {
  // Local state for LIST field input
  const [listInputValue, setListInputValue] = useState('');
  const [isInitialized, setIsInitialized] = useState(false);

  // Initialize local state from form data ONLY when component mounts or field type changes
  useEffect(() => {
    if (condition.fieldType === RULE_FIELD_TYPES.LIST && !isInitialized) {
      const items = condition.items || [];
      const stringValue = items.map(item =>
          item.valueNumber !== null ? item.valueNumber : item.valueString
      ).join(', ');
      setListInputValue(stringValue);
      setIsInitialized(true);
    } else if (condition.fieldType !== RULE_FIELD_TYPES.LIST) {
      setIsInitialized(false);
      setListInputValue('');
    }
  }, [condition.fieldType, isInitialized]);

  const handleUpdate = (field, value) => {
    onUpdate(index, field, value);
  };

  const resetValues = () => {
    handleUpdate('valueString', null);
    handleUpdate('valueNumber', null);
    handleUpdate('valueBoolean', null);
    handleUpdate('items', []);
    setListInputValue('');
    setIsInitialized(false);
  };

  const handleFieldChange = (event) => {
    const newField = event.target.value;
    handleUpdate('field', newField);
  };

  const handleFieldTypeChange = (event) => {
    const newFieldType = event.target.value;
    handleUpdate('fieldType', newFieldType);

    // Set operator to first available value for the new field type
    const availableOperators = VALID_OPERATORS[newFieldType] || [];
    const firstOperator = availableOperators[0] || '';
    handleUpdate('operator', firstOperator);

    // Reset values when field type changes
    resetValues();
  };

  const handleOperatorChange = (event) => {
    const newOperator = event.target.value;
    handleUpdate('operator', newOperator);

    // Reset values when operator changes
    resetValues();
  };

  const handleListInputChange = (event) => {
    const value = event.target.value;

    // Update local state immediately for smooth typing
    setListInputValue(value);

    // Parse and update form data
    const values = value.split(',').map(v => v.trim()).filter(v => v);
    const items = values.map(val => {
      const numValue = parseFloat(val);
      if (!isNaN(numValue) && isFinite(numValue)) {
        return { valueString: null, valueNumber: numValue };
      } else {
        return { valueString: val, valueNumber: null };
      }
    });

    // Clear other fields and update items
    handleUpdate('valueString', null);
    handleUpdate('valueNumber', null);
    handleUpdate('valueBoolean', null);
    handleUpdate('items', items);
  };

  const handleValueChange = (event) => {
    const value = event.target.value;
    const fieldType = condition.fieldType;

    // Clear all values first
    resetValues();

    // For non-LIST field types, set appropriate single value field
    switch (fieldType) {
      case RULE_FIELD_TYPES.BOOLEAN:
        handleUpdate('valueBoolean', value === 'true'); // Store as actual boolean
        break;
      case RULE_FIELD_TYPES.NUMBER:
        handleUpdate('valueNumber', parseFloat(value) || null);
        break;
      case RULE_FIELD_TYPES.STRING:
      default:
        handleUpdate('valueString', value);
        break;
    }
  };

  const renderValueInput = () => {
    const fieldType = condition.fieldType;
    const operator = condition.operator;

    if (!fieldType || !operator) return null;

    if (fieldType === RULE_FIELD_TYPES.BOOLEAN) {
      const currentValue = condition.valueBoolean !== null ? condition.valueBoolean.toString() : '';
      return (
          <FormControl fullWidth size="small" error={!!getError(index, 'valueBoolean')}>
            <InputLabel>Value</InputLabel>
            <Select
                variant="outlined"
                value={currentValue}
                label="Value"
                onChange={handleValueChange}
            >
              <MenuItem value="true">true</MenuItem>
              <MenuItem value="false">false</MenuItem>
            </Select>
            {getError(index, 'valueBoolean') && (
                <FormHelperText>{getError(index, 'valueBoolean')}</FormHelperText>
            )}
          </FormControl>
      );
    }

    if (fieldType === RULE_FIELD_TYPES.NUMBER) {
      const currentValue = condition.valueNumber?.toString() || '';
      return (
          <TextField
              fullWidth
              type="number"
              label="Value"
              value={currentValue}
              onChange={handleValueChange}
              error={!!getError(index, 'valueNumber')}
              helperText={getError(index, 'valueNumber')}
              size="small"
          />
      );
    }

    if (fieldType === RULE_FIELD_TYPES.LIST) {
      return (
          <TextField
              fullWidth
              type="text"
              label="Values (comma-separated)"
              value={listInputValue}
              onChange={handleListInputChange}
              error={!!getError(index, 'items')}
              helperText={getError(index, 'items') || "Enter mixed values separated by commas (numbers and strings)"}
              size="small"
              multiline
              rows={2}
          />
      );
    }

    // STRING field type
    const currentValue = condition.valueString || '';
    return (
        <TextField
            fullWidth
            type="text"
            label="Value"
            value={currentValue}
            onChange={handleValueChange}
            error={!!getError(index, 'valueString')}
            helperText={getError(index, 'valueString')}
            size="small"
        />
    );
  };

  const availableOperators = condition.fieldType
      ? VALID_OPERATORS[condition.fieldType] || []
      : [];

  return (
      <Box sx={{ border: 1, borderColor: 'divider', borderRadius: 1, p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
          <Box sx={{ typography: 'subtitle2' }}>Condition #{index + 1}</Box>
          <IconButton
              size="small"
              onClick={() => onRemove(index)}
              color="error"
          >
            <DeleteIcon />
          </IconButton>
        </Box>

        <Grid container spacing={2}>
          {/* Field Name */}
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField
                fullWidth
                label="Field"
                value={condition.field || ''}
                onChange={handleFieldChange}
                error={!!getError(index, 'field')}
                helperText={getError(index, 'field') || "Field name to evaluate"}
                size="small"
                placeholder="e.g., userId, country, version"
            />
          </Grid>

          {/* Field Type */}
          <Grid size={{ xs: 12, md: 6 }}>
            <FormControl fullWidth size="small" error={!!getError(index, 'fieldType')}>
              <InputLabel>Field Type</InputLabel>
              <Select
                  variant='outlined'
                  value={condition.fieldType || ''}
                  label="Field Type"
                  onChange={handleFieldTypeChange}
              >
                <MenuItem value={RULE_FIELD_TYPES.BOOLEAN}>Boolean</MenuItem>
                <MenuItem value={RULE_FIELD_TYPES.STRING}>String</MenuItem>
                <MenuItem value={RULE_FIELD_TYPES.NUMBER}>Number</MenuItem>
                <MenuItem value={RULE_FIELD_TYPES.LIST}>List</MenuItem>
              </Select>
              {getError(index, 'fieldType') && (
                  <FormHelperText>{getError(index, 'fieldType')}</FormHelperText>
              )}
            </FormControl>
          </Grid>

          {/* Operator */}
          <Grid size={{ xs: 12, md: 6 }}>
            <FormControl fullWidth size="small" error={!!getError(index, 'operator')} disabled={!condition.fieldType}>
              <InputLabel>Operator</InputLabel>
              <Select
                  variant='outlined'
                  value={condition.operator || ''}
                  label="Operator"
                  onChange={handleOperatorChange}
              >
                {availableOperators.map(operator => (
                    <MenuItem key={operator} value={operator}>
                      {operator}
                    </MenuItem>
                ))}
              </Select>
              {getError(index, 'operator') && (
                  <FormHelperText>{getError(index, 'operator')}</FormHelperText>
              )}
            </FormControl>
          </Grid>

          {/* Value */}
          <Grid size={{ xs: 12, md: 6 }}>
            {renderValueInput()}
          </Grid>
        </Grid>
      </Box>
  );
};

export default ConditionCard;
