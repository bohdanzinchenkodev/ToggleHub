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

/**
 * Rule field types for conditions
 */
export const RULE_FIELD_TYPES = {
  BOOLEAN: 'Boolean',
  STRING: 'String', 
  NUMBER: 'Number',
  LIST: 'List'
};

/**
 * Operator types for conditions
 */
export const OPERATOR_TYPES = {
  EQUALS: 'Equals',
  NOT_EQUALS: 'NotEquals',
  GREATER_THAN: 'GreaterThan',
  LESS_THAN: 'LessThan',
  CONTAINS: 'Contains',
  STARTS_WITH: 'StartsWith',
  ENDS_WITH: 'EndsWith',
  IN: 'In',
  NOT_IN: 'NotIn'
};

/**
 * Valid operators for each field type
 */
export const VALID_OPERATORS = {
  [RULE_FIELD_TYPES.BOOLEAN]: [OPERATOR_TYPES.EQUALS, OPERATOR_TYPES.NOT_EQUALS],
  [RULE_FIELD_TYPES.STRING]: [
    OPERATOR_TYPES.EQUALS, 
    OPERATOR_TYPES.NOT_EQUALS, 
    OPERATOR_TYPES.CONTAINS, 
    OPERATOR_TYPES.STARTS_WITH, 
    OPERATOR_TYPES.ENDS_WITH
  ],
  [RULE_FIELD_TYPES.NUMBER]: [
    OPERATOR_TYPES.EQUALS, 
    OPERATOR_TYPES.NOT_EQUALS, 
    OPERATOR_TYPES.GREATER_THAN, 
    OPERATOR_TYPES.LESS_THAN
  ],
  [RULE_FIELD_TYPES.LIST]: [OPERATOR_TYPES.IN, OPERATOR_TYPES.NOT_IN]
};
