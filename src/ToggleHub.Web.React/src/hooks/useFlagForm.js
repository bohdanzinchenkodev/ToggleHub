import { useCallback } from 'react';
import { useFormHandler } from './useFormHandler';
import { useRuleSetManager } from './useRuleSetManager';
import { useFlagDefaults } from './useFlagDefaults';
import { RETURN_VALUE_TYPES, FORM_FIELDS } from '../constants/flagConstants';

/**
 * Main hook for flag form management (create/update)
 * Combines form handling, RuleSet management, and default value logic
 */
export const useFlagForm = (initialData = {}) => {
  const defaultFormData = {
    [FORM_FIELDS.KEY]: '',
    [FORM_FIELDS.DESCRIPTION]: '',
    [FORM_FIELDS.ENABLED]: false,
    [FORM_FIELDS.RETURN_VALUE_TYPE]: RETURN_VALUE_TYPES.BOOLEAN,
    [FORM_FIELDS.DEFAULT_VALUE_ON_RAW]: 'true',
    [FORM_FIELDS.DEFAULT_VALUE_OFF_RAW]: 'false',
    [FORM_FIELDS.RULE_SETS]: [],
    ...initialData
  };

  const {
    formData,
    formErrors,
    handleInputChange: baseHandleInputChange,
    handleServerErrors,
    resetForm,
    setErrors,
    setFormData
  } = useFormHandler(defaultFormData);

  const ruleSetManager = useRuleSetManager(formData, setFormData, formErrors, setErrors);
  const flagDefaults = useFlagDefaults(setFormData, setErrors);

  // Enhanced input change handler
  const handleInputChange = useCallback((event) => {
    const field = event.target.name;
    const value = event.target.type === 'checkbox' ? event.target.checked : event.target.value;

    // Use the base handler for all inputs
    baseHandleInputChange(event);

    // Handle return type changes
    if (field === FORM_FIELDS.RETURN_VALUE_TYPE) {
      flagDefaults.handleReturnTypeChange(value, ruleSetManager.updateRuleSetsForTypeChange);
    }

    // Special handling for Boolean type default values
    flagDefaults.handleBooleanDefaultChange(field, value, formData, setFormData);
  }, [baseHandleInputChange, flagDefaults, formData, setFormData, ruleSetManager]);

  // Dedicated handler for return type changes
  const handleReturnTypeChange = useCallback((newReturnType) => {
    flagDefaults.handleReturnTypeChange(newReturnType, ruleSetManager.updateRuleSetsForTypeChange);
  }, [flagDefaults, ruleSetManager]);

  // Form validation
  const validateForm = useCallback(() => {
    const newErrors = {};

    if (!formData.key.trim()) {
      newErrors.key = 'Flag key is required';
    } else if (!/^[a-zA-Z0-9_-]+$/.test(formData.key)) {
      newErrors.key = 'Flag key can only contain letters, numbers, underscores, and hyphens';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'Description is required';
    }

    if (!formData.returnValueType) {
      newErrors.returnValueType = 'Flag type is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }, [formData, setErrors]);

  // Prepare flag data for submission
  const prepareFlagData = useCallback(() => {
    return {
      projectId: 0, // Will be set by the API call
      environmentId: 0, // Will be set by the API call
      key: formData.key,
      description: formData.description,
      enabled: formData.enabled,
      returnValueType: formData.returnValueType,
      defaultValueOnRaw: formData.defaultValueOnRaw,
      defaultValueOffRaw: formData.defaultValueOffRaw,
      ruleSets: formData.ruleSets.map(ruleSet => ({
        ...(ruleSet.id && { id: ruleSet.id }), // Include id if it exists (for updates)
        returnValueRaw: ruleSet.returnValueRaw,
        offReturnValueRaw: ruleSet.offReturnValueRaw,
        priority: ruleSet.priority,
        percentage: ruleSet.percentage,
        conditions: ruleSet.conditions.map(condition => ({
          ...(condition.id && { id: condition.id }), // Include id if it exists (for updates)
          items: (condition.items || []).map(item => ({
            ...(item.id && { id: item.id }), // Include id if it exists (for updates)
            valueString: item.valueString,
            valueNumber: item.valueNumber
          })),
          field: condition.field,
          fieldType: condition.fieldType,
          operator: condition.operator,
          valueString: condition.valueString,
          valueNumber: condition.valueNumber,
          valueBoolean: condition.valueBoolean
        }))
      }))
    };
  }, [formData]);

  return {
    formData,
    formErrors,
    handleInputChange,
    handleReturnTypeChange,
    handleServerErrors,
    validateForm,
    prepareFlagData,
    resetForm,
    setFormData,
    setErrors,
    ruleSetManager
  };
};
