import { useSelector } from 'react-redux';
import { selectCurrentUserPermissions } from '../redux/slices/appStateSlice';

export const usePermissions = () => {
  const permissions = useSelector(selectCurrentUserPermissions);

  const hasPermission = (permission) => {
    if (!permissions || !Array.isArray(permissions)) return false;
    return permissions.includes(permission);
  };

  return {
    permissions,
    hasPermission,
  };
};
