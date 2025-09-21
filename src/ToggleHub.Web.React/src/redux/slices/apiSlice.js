import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const api = createApi({
	reducerPath: 'api', // key in the store
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5160/api',
		credentials: 'include', // send cookies automatically
	}),
	tagTypes: ['Flags'],
	endpoints: (builder) => ({
		getUser: builder.query({
			query: () => 'user/me',
		}),
		login: builder.mutation({
			query: (body) => ({
				url: 'auth/login',
				method: 'POST',
				body,
			}),
		}),
		register: builder.mutation({
			query: (body) => ({
				url: 'auth/register',
				method: 'POST',
				body,
			}),
		}),
		getOrganizationsByCurrentUser: builder.query({
			query: () => 'user/me/organizations',
		}),
		createOrganization: builder.mutation({
			query: (body) => ({
				url: 'organizations',
				method: 'POST',
				body,
			}),
		}),
		getProjectsByOrganization: builder.query({
			query: (organizationId) => `organizations/${organizationId}/projects`,
		}),
		getOrganizationBySlug: builder.query({
			query: (slug) => `organizations/${slug}`,
		}),
		createProject: builder.mutation({
			query: ({ organizationId, body }) => ({
				url: `organizations/${organizationId}/projects`,
				method: 'POST',
				body,
			}),
		}),
		getProjectBySlug: builder.query({
			query: ({ projectSlug, organizationId }) =>
				`organizations/${organizationId}/projects/${projectSlug}`,
		}),
		updateProject: builder.mutation({
			query: ({ organizationId, projectId, body }) => ({
				url: `organizations/${organizationId}/projects/${projectId}`,
				method: 'PUT',
				body,
			}),
		}),
		deleteProject: builder.mutation({
			query: ({ organizationId, projectId }) => ({
				url: `organizations/${organizationId}/projects/${projectId}`,
				method: 'DELETE',
			}),
		}),
		getFlagsByEnvironment: builder.query({
			query: ({ organizationId, projectId, environmentId }) =>
				`organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags`,
			providesTags: (result, error, { organizationId, projectId, environmentId }) => [
				{ type: 'Flags', id: `${organizationId}-${projectId}-${environmentId}` }
			],
		}),
		enableFlag: builder.mutation({
			query: ({ organizationId, projectId, environmentId, flagId }) => ({
				url: `organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags/${flagId}/enable`,
				method: 'PATCH',
			}),
		}),
		disableFlag: builder.mutation({
			query: ({ organizationId, projectId, environmentId, flagId }) => ({
				url: `organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags/${flagId}/disable`,
				method: 'PATCH',
			}),
		}),
		createFlag: builder.mutation({
			query: ({ organizationId, projectId, environmentId, body }) => ({
				url: `organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags`,
				method: 'POST',
				body,
			}),
			invalidatesTags: (result, error, { organizationId, projectId, environmentId }) => [
				{ type: 'Flags', id: `${organizationId}-${projectId}-${environmentId}` }
			],
		}),
		updateFlag: builder.mutation({
			query: ({ organizationId, projectId, environmentId, flagId, body }) => ({
				url: `organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags`,
				method: 'PUT',
				body: { ...body, id: flagId },
			}),
			invalidatesTags: (result, error, { organizationId, projectId, environmentId, flagId }) => [
				{ type: 'Flags', id: `${organizationId}-${projectId}-${environmentId}` },
				{ type: 'Flag', id: `${organizationId}-${projectId}-${environmentId}-${flagId}` }
			],
		}),
		getFlagById: builder.query({
			query: ({ organizationId, projectId, environmentId, flagId }) =>
				`organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags/${flagId}`,
			providesTags: (result, error, { organizationId, projectId, environmentId, flagId }) => [
				{ type: 'Flag', id: `${organizationId}-${projectId}-${environmentId}-${flagId}` }
			],
		}),
		getApiKeys: builder.query({
			query: ({ organizationId, projectId, environmentId }) =>
				`organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/apikeys`,
		}),
	}),
});

export const {
	useGetUserQuery,
	useLoginMutation,
	useRegisterMutation,
	useGetOrganizationsByCurrentUserQuery,
	useCreateOrganizationMutation,
	useGetProjectsByOrganizationQuery,
	useGetOrganizationBySlugQuery,
	useCreateProjectMutation,
	useGetProjectBySlugQuery,
	useUpdateProjectMutation,
	useDeleteProjectMutation,
	useGetFlagsByEnvironmentQuery,
	useEnableFlagMutation,
	useDisableFlagMutation,
	useCreateFlagMutation,
	useUpdateFlagMutation,
	useGetFlagByIdQuery,
	useGetApiKeysQuery
} = api;
