import { useCallback } from 'react';
import { getDefaultValuesForType } from '../utils/flagUtils';

/**
 * Custom hook for managing flag default values based on return type
 */
export const useFlagDefaults = (setFormData, setErrors) => {

  const handleReturnTypeChange = useCallback((newReturnType, updateRuleSetsCallback) => {
    const { defaultOn, defaultOff } = getDefaultValuesForType(newReturnType);

    setFormData(prev => ({
      ...prev,
      returnValueType: newReturnType,
      defaultValueOnRaw: defaultOn,
      defaultValueOffRaw: defaultOff
    }));

    // Update existing ruleSets if callback provided
    if (updateRuleSetsCallback) {
      updateRuleSetsCallback(newReturnType);
    }

    // Clear errors for default values when return type changes
    setErrors(prev => ({
      ...prev,
      defaultValueOnRaw: '',
      defaultValueOffRaw: ''
    }));
  }, [setFormData, setErrors]);

  const handleBooleanDefaultChange = useCallback((field, value, formData, setFormData) => {
    // For Boolean type, automatically set the opposite value
    if (formData.returnValueType === 'Boolean') {
      const oppositeField = field === 'defaultValueOnRaw' ? 'defaultValueOffRaw' : 'defaultValueOnRaw';
      const oppositeValue = value === 'true' ? 'false' : 'true';

      setFormData(prev => ({
        ...prev,
        [oppositeField]: oppositeValue
      }));
    }
  }, []);

  return {
    handleReturnTypeChange,
    handleBooleanDefaultChange
  };
};
