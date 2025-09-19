
import { RETURN_VALUE_TYPES, DEFAULT_VALUES } from '../constants/flagConstants';

export const getDefaultValuesForType = (returnType) => {
  return DEFAULT_VALUES[returnType] || DEFAULT_VALUES[RETURN_VALUE_TYPES.STRING];
};

/**
 * Get the default value for a specific type and state (on/off)
 */
export const getDefaultValueForTypeAndState = (returnType, isOffValue = false) => {
  const { defaultOn, defaultOff } = getDefaultValuesForType(returnType);
  return isOffValue ? defaultOff : defaultOn;
};
