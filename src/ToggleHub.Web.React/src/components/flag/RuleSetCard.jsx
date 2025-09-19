import React from 'react';
import {
  Box,
  Typography,
  TextField,
  IconButton,
  Grid,
  Card,
  CardContent
} from '@mui/material';
import { Delete as DeleteIcon } from '@mui/icons-material';
import ReturnValueInput from './ReturnValueInput';
import { RETURN_VALUE_TYPES, RULESET_FIELDS } from '../../constants/flagConstants';

/**
 * Component for individual RuleSet management
 */
const RuleSetCard = ({ 
  ruleSet, 
  index, 
  returnValueType, 
  onUpdate, 
  onRemove, 
  getError 
}) => {
  
  const handleUpdate = (field, value) => {
    onUpdate(index, field, value);
  };

  const handleReturnValueChange = (field) => (event) => {
    const newValue = event.target.value;
    handleUpdate(field, newValue);
    
    // For Boolean types, automatically set the opposite value
    if (returnValueType === RETURN_VALUE_TYPES.BOOLEAN) {
      const oppositeField = field === RULESET_FIELDS.RETURN_VALUE_RAW ? RULESET_FIELDS.OFF_RETURN_VALUE_RAW : RULESET_FIELDS.RETURN_VALUE_RAW;
      const oppositeValue = newValue === 'true' ? 'false' : 'true';
      handleUpdate(oppositeField, oppositeValue);
    }
  };

  return (
    <Card
      variant="outlined"
      sx={{
        bgcolor: 'transparent',
        backgroundColor: 'transparent'
      }}
    >
      <CardContent sx={{ pb: 1 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
          <Typography variant="subtitle2">Rule Set #{index + 1}</Typography>
          <IconButton
            size="small"
            onClick={() => onRemove(index)}
            color="error"
          >
            <DeleteIcon />
          </IconButton>
        </Box>

        <Grid container spacing={2}>
          {/* Priority */}
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField
              fullWidth
              type="number"
              name={`ruleSets[${index}].priority`}
              label="Priority"
              value={ruleSet.priority}
              onChange={(e) => handleUpdate('priority', parseInt(e.target.value) || 1)}
              error={!!getError(index, 'priority')}
              helperText={getError(index, 'priority')}
              size="small"
            />
          </Grid>

          {/* Percentage */}
          <Grid size={{ xs: 12, md: 6 }}>
            <TextField
              fullWidth
              type="number"
              name={`ruleSets[${index}].percentage`}
              label="Percentage"
              value={ruleSet.percentage}
              onChange={(e) => handleUpdate('percentage', parseInt(e.target.value) || 100)}
              error={!!getError(index, 'percentage')}
              helperText={getError(index, 'percentage')}
              size="small"
            />
          </Grid>

          {/* Return Value ON */}
          <Grid size={{ xs: 12, md: 6 }}>
            <ReturnValueInput
              returnValueType={returnValueType}
              value={ruleSet.returnValueRaw}
              onChange={handleReturnValueChange(RULESET_FIELDS.RETURN_VALUE_RAW)}
              name={`ruleSets[${index}].${RULESET_FIELDS.RETURN_VALUE_RAW}`}
              label="Return Value (ON)"
              isOffValue={false}
              error={getError(index, RULESET_FIELDS.RETURN_VALUE_RAW)}
            />
          </Grid>

          {/* Return Value OFF */}
          <Grid size={{ xs: 12, md: 6 }}>
            <ReturnValueInput
              returnValueType={returnValueType}
              value={ruleSet.offReturnValueRaw}
              onChange={handleReturnValueChange(RULESET_FIELDS.OFF_RETURN_VALUE_RAW)}
              name={`ruleSets[${index}].${RULESET_FIELDS.OFF_RETURN_VALUE_RAW}`}
              label="Return Value (OFF)"
              isOffValue={true}
              error={getError(index, RULESET_FIELDS.OFF_RETURN_VALUE_RAW)}
            />
          </Grid>
        </Grid>

        {/* Conditions placeholder */}
        <Box sx={{ mt: 2, p: 2, borderRadius: 1 }}>
          <Typography variant="body2" color="text.secondary">
            Conditions will be added in the next step
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
};

export default RuleSetCard;
