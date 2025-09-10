import { useEffect, useState } from "react";

export function useAuth() {
	const [user, setUser] = useState({id: 1});
	const [loading, setLoading] = useState(false);

	/*useEffect(() => {
		fetch("/api/auth/me", { credentials: "include" })
			.then(res => {
				if (res.status === 401) {
					setUser(null);
				} else {
					return res.json();
				}
			})
			.then(data => {
				if (data) setUser(data);
			})
			.finally(() => setLoading(false));
	}, []);*/

	return { user, loading };
}
