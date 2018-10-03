import { Api } from './Api'

describe('Api tests', () => {

	let fetch
	let fixture

	beforeEach(() => {
		fetch = window.fetch
		fixture = new Api()
	})

	afterEach(() => {
		window.fetch = fetch
	})

	it('should get user data', () => {
		const mockFetch = jest.fn(() => Promise.resolve(new Response(JSON.stringify({ prop: 'data' }))))
		window.fetch = mockFetch

		fixture.getUserData().then(data => expect(data).toEqual({ prop: 'data' }))
		expect(mockFetch).toHaveBeenCalledTimes(1)
		expect(mockFetch).toHaveBeenCalledWith('/api/user/data', { method: 'GET', credentials: 'same-origin' })
	})

	it('should post user logout', () => {
		const mockFetch = jest.fn(() => Promise.resolve())
		window.fetch = mockFetch

		fixture.postUserLogout()
		expect(mockFetch).toHaveBeenCalledTimes(1)
		expect(mockFetch).toHaveBeenCalledWith('/api/user/logout', { method: 'POST', credentials: 'same-origin' })
	})

	it('should post create assignment request', () => {
		const mockFetch = jest.fn(() => Promise.resolve(new Response(JSON.stringify({ prop: 'response data' }))))
		window.fetch = mockFetch

		fixture.postCreateAssignmentRequest({ prop: 'request data' })
			.then(data => expect(data).toEqual({ prop: 'response data' }))
		expect(mockFetch).toHaveBeenCalledTimes(1)
		expect(mockFetch.mock.calls[0][0]).toBe('/api/fnol')
		expect(mockFetch.mock.calls[0][1]).toEqual({
			method: 'POST',
			credentials: 'same-origin',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({ prop: 'request data' })
		})
	})
})