import { useNavigate, useSearchParams } from 'react-router';

export const useAuthRedirect = () => {
	const navigate = useNavigate();
	const [searchParams] = useSearchParams();
	const returnUrl = searchParams.get('returnUrl');

	const isSafeReturnUrl = (url) => {
		if (!url) return false;
		// Prevent redirects to login/register pages
		if (url.includes('/login') || url.includes('/register')) return false;
		// Only allow relative URLs (starting with /) or same domain
		if (url.startsWith('/')) return true;
		try {
			const returnUrlObj = new URL(url, window.location.origin);
			return returnUrlObj.origin === window.location.origin;
		} catch {
			return false;
		}
	};

	const redirectAfterAuth = () => {
		const shouldRedirectToReturnUrl = isSafeReturnUrl(returnUrl);
		navigate(shouldRedirectToReturnUrl ? returnUrl : "/");
	};

	const getAuthLinkUrl = (authType) => {
		const oppositeType = authType === 'login' ? 'register' : 'login';
		return `/${oppositeType}${returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : ''}`;
	};

	return {
		returnUrl,
		redirectAfterAuth,
		getAuthLinkUrl
	};
};
