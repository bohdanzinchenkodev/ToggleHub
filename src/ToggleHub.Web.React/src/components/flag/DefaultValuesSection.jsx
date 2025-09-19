import React from 'react';
import { Box, Typography, Grid } from '@mui/material';
import ReturnValueInput from './ReturnValueInput';

/**
 * Component for managing default values based on return type
 */
const DefaultValuesSection = ({
  returnValueType,
  defaultValueOnRaw,
  defaultValueOffRaw,
  onChange,
  errors = {}
}) => {

  return (
    <Box sx={{ mb: 3 }}>
      <Typography variant="subtitle1" sx={{ mb: 2 }}>Default Values</Typography>

      <Grid container spacing={2}>
        <Grid size={{ xs: 12, md: 6 }}>
          <ReturnValueInput
            returnValueType={returnValueType}
            value={defaultValueOnRaw}
            onChange={onChange}
            name="defaultValueOnRaw"
            label="Default Value (ON)"
            isOffValue={false}
            error={errors.defaultValueOnRaw}
            helperText={!errors.defaultValueOnRaw ? `${returnValueType} value returned when flag is enabled` : undefined}
          />
        </Grid>

        <Grid size={{ xs: 12, md: 6 }}>
          <ReturnValueInput
            returnValueType={returnValueType}
            value={defaultValueOffRaw}
            onChange={onChange}
            name="defaultValueOffRaw"
            label="Default Value (OFF)"
            isOffValue={true}
            error={errors.defaultValueOffRaw}
            helperText={!errors.defaultValueOffRaw ? `${returnValueType} value returned when flag is disabled` : undefined}
          />
        </Grid>
      </Grid>
    </Box>
  );
};

export default DefaultValuesSection;
