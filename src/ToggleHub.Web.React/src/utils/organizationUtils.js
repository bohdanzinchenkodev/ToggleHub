import { CHIP_COLORS } from '../constants/organizationConstants';

/**
 * Gets the appropriate chip color for a given value
 * @param {string} value - The value to get color for (role or status)
 * @returns {string} - Material-UI color name
 */
export const getChipColor = (value) => CHIP_COLORS[value] || 'default';
