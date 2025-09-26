import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const api = createApi({
	reducerPath: 'api', // key in the store
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5160/api',
		credentials: 'include', // send cookies automatically
	}),
	tagTypes: ['Flags', 'User', 'UserPermissions', 'OrganizationInvites', 'OrganizationMembers', 'Organizations', 'Projects'],
	endpoints: (builder) => ({
		getUser: builder.query({
			query: () => 'user/me',
			providesTags: ['User'],
		}),
		updateUser: builder.mutation({
			query: (body) => ({
				url: 'user/me',
				method: 'PUT',
				body,
			}),
			invalidatesTags: ['User'],
		}),
		getUserPermissions: builder.query({
			query: (organizationId) => `user/me/permissions/${organizationId}`,
			providesTags: (result, error, organizationId) => [
				{ type: 'UserPermissions', id: organizationId }
			],
		}),
		login: builder.mutation({
			query: (body) => ({
				url: 'auth/login',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['User'],
		}),
		register: builder.mutation({
			query: (body) => ({
				url: 'auth/register',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['User'],
		}),
		logout: builder.mutation({
			query: () => ({
				url: 'auth/logout',
				method: 'POST',
			}),
			async onQueryStarted(arg, { dispatch, queryFulfilled }) {
				try {
					await queryFulfilled;
					// 🚀 Reset the entire API slice cache
					dispatch(api.util.resetApiState());
				} catch {}
			},
		}),
		forgotPassword: builder.mutation({
			query: (body) => ({
				url: 'auth/forgot-password',
				method: 'POST',
				body,
			}),
		}),
		resetPassword: builder.mutation({
			query: (body) => ({
				url: 'auth/reset-password',
				method: 'POST',
				body,
			}),
		}),
		getOrganizationsByCurrentUser: builder.query({
			query: ({ page = 1, pageSize = 25 } = {}) => ({
				url: 'user/me/organizations',
				params: {
					page,
					pageSize
				}
			}),
			providesTags: ['Organizations'],
		}),
		createOrganization: builder.mutation({
			query: (body) => ({
				url: 'organizations',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Organizations'],
		}),
		getProjectsByOrganization: builder.query({
			query: ({ organizationId, page = 1, pageSize = 25 }) => ({
				url: `organizations/${organizationId}/projects`,
				params: {
					page,
					pageSize
				}
			}),
			providesTags: (result, error, { organizationId }) => [
				{ type: 'Projects', id: organizationId }
			],
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
			invalidatesTags: (result, error, { organizationId }) => [
				{ type: 'Projects', id: organizationId }
			],
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
			invalidatesTags: (result, error, { organizationId }) => [
				{ type: 'Projects', id: organizationId }
			],
		}),
		deleteProject: builder.mutation({
			query: ({ organizationId, projectId }) => ({
				url: `organizations/${organizationId}/projects/${projectId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, { organizationId }) => [
				{ type: 'Projects', id: organizationId }
			],
		}),
		deleteOrganization: builder.mutation({
			query: ({ organizationId }) => ({
				url: `organizations/${organizationId}`,
				method: 'DELETE',
			}),
			invalidatesTags: ['Organizations'],
		}),
		getFlagsByEnvironment: builder.query({
			query: ({ organizationId, projectId, environmentId, page = 1, pageSize = 10 }) =>
				`organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags?page=${page}&pageSize=${pageSize}`,
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
			query: ({ organizationId, projectId, environmentId, page = 1, pageSize = 25 }) =>
				`organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/apikeys?page=${page}&pageSize=${pageSize}`,
		}),
		sendOrganizationInvite: builder.mutation({
			query: ({ organizationId, body }) => ({
				url: `organizations/${organizationId}/invites`,
				method: 'POST',
				body,
			}),
			invalidatesTags: ['OrganizationInvites'],
		}),
		resendOrganizationInvite: builder.mutation({
			query: ({ organizationId, inviteId }) => ({
				url: `organizations/${organizationId}/invites/resend/${inviteId}`,
				method: 'POST',
			}),
			invalidatesTags: ['OrganizationInvites'],
		}),
		revokeOrganizationInvite: builder.mutation({
			query: ({ organizationId, inviteId }) => ({
				url: `organizations/${organizationId}/invites/revoke/${inviteId}`,
				method: 'POST',
			}),
			invalidatesTags: ['OrganizationInvites'],
		}),
		getOrganizationInvites: builder.query({
			query: ({ organizationId, page = 1, pageSize = 10 }) => ({
				url: `organizations/${organizationId}/invites`,
				params: {
					page,
					pageSize,
				},
			}),
			providesTags: ['OrganizationInvites'],
		}),
		getOrganizationMembers: builder.query({
			query: ({ organizationId, page = 1, pageSize = 10 }) => ({
				url: `organizations/${organizationId}/members`,
				params: {
					page,
					pageSize,
				},
			}),
			providesTags: ['OrganizationMembers'],
		}),
		updateOrganizationMemberRole: builder.mutation({
			query: ({ organizationId, userId, newRole }) => ({
				url: `organizations/${organizationId}/members/${userId}/role`,
				method: 'PATCH',
				body: { newRole },
			}),
			invalidatesTags: ['OrganizationMembers'],
		}),
		deleteOrganizationMember: builder.mutation({
			query: ({ organizationId, orgMemberId }) => ({
				url: `organizations/${organizationId}/members/${orgMemberId}`,
				method: 'DELETE',
			}),
			invalidatesTags: ['OrganizationMembers'],
		}),
		acceptOrganizationInvite: builder.mutation({
			query: ({ organizationId, body }) => ({
				url: `organizations/${organizationId}/invites/accept`,
				method: 'POST',
				body,
			}),
		}),
		declineOrganizationInvite: builder.mutation({
			query: ({ organizationId, token }) => ({
				url: `organizations/${organizationId}/invites/decline/${token}`,
				method: 'POST',
			}),
		}),
		deleteFlag: builder.mutation({
			query: ({ organizationId, projectId, environmentId, flagId }) => ({
				url: `organizations/${organizationId}/projects/${projectId}/environments/${environmentId}/flags/${flagId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, { organizationId, projectId, environmentId }) => [
				{ type: 'Flags', id: `${organizationId}-${projectId}-${environmentId}` }
			],
		}),
	}),
});

export const {
	useGetUserQuery,
	useGetUserPermissionsQuery,
	useUpdateUserMutation,
	useLoginMutation,
	useLogoutMutation,
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
	useGetApiKeysQuery,
	useSendOrganizationInviteMutation,
	useResendOrganizationInviteMutation,
	useRevokeOrganizationInviteMutation,
	useGetOrganizationInvitesQuery,
	useGetOrganizationMembersQuery,
	useUpdateOrganizationMemberRoleMutation,
	useDeleteOrganizationMemberMutation,
	useAcceptOrganizationInviteMutation,
	useDeclineOrganizationInviteMutation,
	useDeleteFlagMutation,
	useDeleteOrganizationMutation,
	useForgotPasswordMutation,
	useResetPasswordMutation
} = api;
