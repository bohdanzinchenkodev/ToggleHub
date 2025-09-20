import React from 'react';
import {
  Box,
  Typography,
  Button,
  Collapse
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import ConditionCard from './ConditionCard';

/**
 * Component for managing rule conditions within a RuleSet
 */
const ConditionsManager = ({
  conditions = [],
  onAdd,
  onUpdate,
  onRemove,
  getError,
  ruleSetIndex
}) => {

  const handleAddCondition = () => {
    const newCondition = {
      field: '',
      fieldTypeString: '',
      operatorString: '',
      valueString: null,
      valueNumber: null
    };
    onAdd(ruleSetIndex, newCondition);
  };

  const handleUpdateCondition = (conditionIndex, field, value) => {
    onUpdate(ruleSetIndex, conditionIndex, field, value);
  };

  const handleRemoveCondition = (conditionIndex) => {
    onRemove(ruleSetIndex, conditionIndex);
  };

  const getConditionError = (conditionIndex, field) => {
    return getError(ruleSetIndex, conditionIndex, field);
  };

  return (
    <Box sx={{ mt: 2 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="subtitle2">
          Conditions ({conditions.length})
        </Typography>
        <Button
          size="small"
          startIcon={<AddIcon />}
          onClick={handleAddCondition}
          variant="outlined"
        >
          Add Condition
        </Button>
      </Box>

      <Collapse in={conditions.length > 0}>
        <Box>
          {conditions.map((condition, index) => (
            <ConditionCard
              key={index}
              condition={condition}
              index={index}
              onUpdate={handleUpdateCondition}
              onRemove={handleRemoveCondition}
              getError={getConditionError}
            />
          ))}
        </Box>
      </Collapse>

      {conditions.length === 0 && (
        <Box
          sx={{
            p: 3,
            textAlign: 'center',
            color: 'text.secondary',
            border: 1,
            borderColor: 'divider',
            borderRadius: 1,
          }}
        >
          <Typography variant="body2">
            No conditions defined. Add conditions to control when this rule applies.
          </Typography>
        </Box>
      )}
    </Box>
  );
};

export default ConditionsManager;
