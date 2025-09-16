import { useDispatch, useSelector } from 'react-redux';
import {
	setCurrentOrganization,
	setCurrentProject,
	clearCurrentOrganization,
	clearCurrentProject,
	clearAppState,
	selectCurrentOrganization,
	selectCurrentProject,
} from '../redux/slices/appStateSlice';

export const useAppState = () => {
	const dispatch = useDispatch();
	const currentOrganization = useSelector(selectCurrentOrganization);
	const currentProject = useSelector(selectCurrentProject);

	const updateCurrentOrganization = (organization) => {
		dispatch(setCurrentOrganization(organization));
	};

	const updateCurrentProject = (project) => {
		dispatch(setCurrentProject(project));
	};

	const clearOrganization = () => {
		dispatch(clearCurrentOrganization());
	};

	const clearProject = () => {
		dispatch(clearCurrentProject());
	};

	const clearAll = () => {
		dispatch(clearAppState());
	};

	return {
		currentOrganization,
		currentProject,
		updateCurrentOrganization,
		updateCurrentProject,
		clearOrganization,
		clearProject,
		clearAll,
	};
};
