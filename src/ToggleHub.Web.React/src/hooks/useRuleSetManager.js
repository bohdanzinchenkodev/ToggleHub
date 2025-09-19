import { useCallback } from 'react';
import { getDefaultValuesForType } from '../utils/flagUtils';
import { RULESET_FIELDS } from '../constants/flagConstants';

/**
 * Custom hook for managing RuleSet operations
 * Provides CRUD operations for rule sets and error handling
 */
export const useRuleSetManager = (formData, setFormData, formErrors, setFormErrors) => {

  const addRuleSet = useCallback(() => {
    setFormData(prev => {
      const { defaultOn, defaultOff } = getDefaultValuesForType(prev.returnValueType);

      const newRuleSet = {
        returnValueRaw: defaultOn,
        offReturnValueRaw: defaultOff,
        priority: prev.ruleSets.length + 1,
        percentage: 100,
        conditions: []
      };

      return {
        ...prev,
        ruleSets: [...prev.ruleSets, newRuleSet]
      };
    });
  }, [setFormData]);

  const removeRuleSet = useCallback((index) => {
    setFormData(prev => ({
      ...prev,
      ruleSets: prev.ruleSets.filter((_, i) => i !== index)
    }));
  }, [setFormData]);

  const updateRuleSet = useCallback((index, field, value) => {
    setFormData(prev => ({
      ...prev,
      ruleSets: prev.ruleSets.map((ruleSet, i) =>
        i === index ? { ...ruleSet, [field]: value } : ruleSet
      )
    }));
  }, [setFormData]);

  const updateRuleSetsForTypeChange = useCallback((newReturnType) => {
    const { defaultOn, defaultOff } = getDefaultValuesForType(newReturnType);

    setFormData(prev => ({
      ...prev,
      ruleSets: prev.ruleSets.map(ruleSet => ({
        ...ruleSet,
        returnValueRaw: defaultOn,
        offReturnValueRaw: defaultOff
      }))
    }));

    // Clear all RuleSet validation errors when return type changes
    setFormErrors(prev => {
      const newErrors = {};
      
      // Keep only errors that are NOT RuleSet return value errors
      Object.keys(prev).forEach(key => {
        const isRuleSetReturnValueError = key.startsWith('ruleSets[') && 
          (key.includes(`.${RULESET_FIELDS.RETURN_VALUE_RAW}`) || key.includes(`.${RULESET_FIELDS.OFF_RETURN_VALUE_RAW}`));
        
        if (!isRuleSetReturnValueError) {
          newErrors[key] = prev[key];
        }
      });
      
      return newErrors;
    });
  }, [setFormData, setFormErrors]);

  const getRuleSetError = useCallback((index, field) => {
    if (!formErrors) return null;
    const errorKey = `ruleSets[${index}].${field}`;
    return formErrors[errorKey] || null;
  }, [formErrors]);

  return {
    addRuleSet,
    removeRuleSet,
    updateRuleSet,
    updateRuleSetsForTypeChange,
    getRuleSetError,
    ruleSets: formData.ruleSets
  };
};
