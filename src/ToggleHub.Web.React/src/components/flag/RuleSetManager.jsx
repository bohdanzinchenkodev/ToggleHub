import React from 'react';
import { Box, Typography, Button } from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import RuleSetCard from './RuleSetCard';

/**
 * Component for managing multiple RuleSets
 */
const RuleSetManager = ({ 
  ruleSets, 
  returnValueType, 
  onAdd, 
  onUpdate, 
  onRemove, 
  getError,
  onAddCondition,
  onUpdateCondition,
  onRemoveCondition,
  getConditionError
}) => {
  
  return (
    <Box sx={{ mb: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'flex-end', mb: 2 }}>
        <Button
          variant="outlined"
          size="small"
          startIcon={<AddIcon />}
          onClick={onAdd}
        >
          Add Rule Set
        </Button>
      </Box>

      {ruleSets.length === 0 ? (
        <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
          No rule sets defined. Rule sets allow you to override default values based on conditions.
        </Typography>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          {ruleSets.map((ruleSet, index) => (
            <RuleSetCard
              key={ruleSet.id || index}
              ruleSet={ruleSet}
              index={index}
              returnValueType={returnValueType}
              onUpdate={onUpdate}
              onRemove={onRemove}
              getError={getError}
              onAddCondition={onAddCondition}
              onUpdateCondition={onUpdateCondition}
              onRemoveCondition={onRemoveCondition}
              getConditionError={getConditionError}
            />
          ))}
        </Box>
      )}
    </Box>
  );
};

export default RuleSetManager;
