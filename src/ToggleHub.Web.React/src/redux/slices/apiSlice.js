import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const api = createApi({
	reducerPath: 'api', // key in the store
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5160/api',
		credentials: 'include', // send cookies automatically
	}),
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
	}),
});

export const { useGetUserQuery, useLoginMutation, useRegisterMutation, useGetOrganizationsByCurrentUserQuery, useCreateOrganizationMutation } = api;
