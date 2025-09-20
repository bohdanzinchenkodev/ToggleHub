import React from 'react';
import {
  Box,
  Typography,
  TextField,
  FormControlLabel,
  Switch,
  Button,
  Alert
} from '@mui/material';
import { Save as SaveIcon } from '@mui/icons-material';
import ReturnTypeSelector from './ReturnTypeSelector';
import DefaultValuesSection from './DefaultValuesSection';
import RuleSetManager from './RuleSetManager';

/**
 * Shared form component for flag creation/editing
 */
const FlagForm = ({
  mode = 'create', // 'create' or 'update'
  formData,
  formErrors,
  isSubmitting = false,
  submitError = null,
  onInputChange,
  onReturnTypeChange,
  onSubmit,
  onCancel,
  ruleSetManager
}) => {

  const handleSubmit = (event) => {
    event.preventDefault();
    onSubmit(event);
  };

  const submitButtonText = mode === 'create' ? 'Create Flag' : 'Update Flag';
  const submittingText = mode === 'create' ? 'Creating...' : 'Updating...';

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ mt: 3 }}>
      {submitError && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {(submitError?.status !== 500 && submitError?.data?.detail) || "Something went wrong. Please try again."}
        </Alert>
      )}

      {/* Flag Key */}
      <TextField
        fullWidth
        name="key"
        label="Flag Key"
        value={formData.key}
        onChange={onInputChange}
        error={!!formErrors.key}
        helperText={formErrors.key || 'Unique identifier for your feature flag (e.g., new-checkout-flow)'}
        sx={{ mb: 3 }}
        size="small"
        required
      />

      {/* Description */}
      <TextField
        fullWidth
        name="description"
        label="Description"
        value={formData.description}
        onChange={onInputChange}
        error={!!formErrors.description}
        helperText={formErrors.description || 'Brief description of what this flag controls'}
        multiline
        rows={3}
        sx={{ mb: 3 }}
        size="small"
        required
      />

      {/* Return Value Type */}
      <ReturnTypeSelector
        value={formData.returnValueType}
        onChange={onReturnTypeChange}
        error={formErrors.returnValueType}
      />

      {/* Default Values */}
      <DefaultValuesSection
        returnValueType={formData.returnValueType}
        defaultValueOnRaw={formData.defaultValueOnRaw}
        defaultValueOffRaw={formData.defaultValueOffRaw}
        onChange={onInputChange}
        errors={formErrors}
      />

      {/* RuleSets */}
      <RuleSetManager
        ruleSets={ruleSetManager.ruleSets}
        returnValueType={formData.returnValueType}
        onAdd={ruleSetManager.addRuleSet}
        onUpdate={ruleSetManager.updateRuleSet}
        onRemove={ruleSetManager.removeRuleSet}
        getError={ruleSetManager.getRuleSetError}
        onAddCondition={ruleSetManager.addCondition}
        onUpdateCondition={ruleSetManager.updateCondition}
        onRemoveCondition={ruleSetManager.removeCondition}
        getConditionError={ruleSetManager.getConditionError}
      />

      {/* Initial State */}
      <FormControlLabel
        control={
          <Switch
            name="enabled"
            checked={formData.enabled}
            onChange={onInputChange}
            color="primary"
            size="small"
          />
        }
        label={
          <Typography variant="body2">
            {mode === 'create' 
              ? 'Enable flag immediately after creation'
              : 'Enable flag'
            }
          </Typography>
        }
        sx={{ mb: 3 }}
      />

      {/* Action Buttons */}
      <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
        <Button
          type="button"
          variant="outlined"
          onClick={onCancel}
          disabled={isSubmitting}
          size="small"
        >
          Cancel
        </Button>
        <Button
          type="submit"
          variant="contained"
          startIcon={<SaveIcon />}
          disabled={isSubmitting}
          size="small"
        >
          {isSubmitting ? submittingText : submitButtonText}
        </Button>
      </Box>
    </Box>
  );
};

export default FlagForm;
