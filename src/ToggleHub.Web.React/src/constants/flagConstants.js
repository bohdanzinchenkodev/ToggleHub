/**
 * Constants for feature flag return value types
 */
export const RETURN_VALUE_TYPES = {
  BOOLEAN: 'Boolean',
  NUMBER: 'Number',
  STRING: 'String',
  JSON: 'JSON'
};

/**
 * Default values for each return type
 */
export const DEFAULT_VALUES = {
  [RETURN_VALUE_TYPES.BOOLEAN]: {
    defaultOn: 'true',
    defaultOff: 'false'
  },
  [RETURN_VALUE_TYPES.NUMBER]: {
    defaultOn: '1',
    defaultOff: '0'
  },
  [RETURN_VALUE_TYPES.STRING]: {
    defaultOn: '',
    defaultOff: ''
  },
  [RETURN_VALUE_TYPES.JSON]: {
    defaultOn: '{}',
    defaultOff: '{}'
  }
};

/**
 * Field names for rule sets
 */
export const RULESET_FIELDS = {
  RETURN_VALUE_RAW: 'returnValueRaw',
  OFF_RETURN_VALUE_RAW: 'offReturnValueRaw',
  PRIORITY: 'priority',
  PERCENTAGE: 'percentage'
};

/**
 * Form field names
 */
export const FORM_FIELDS = {
  KEY: 'key',
  DESCRIPTION: 'description',
  ENABLED: 'enabled',
  RETURN_VALUE_TYPE: 'returnValueType',
  DEFAULT_VALUE_ON_RAW: 'defaultValueOnRaw',
  DEFAULT_VALUE_OFF_RAW: 'defaultValueOffRaw',
  RULE_SETS: 'ruleSets'
};
