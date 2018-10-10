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

	describe('post create assignment request', () => {

		let actual
		let mockFetch
		let postAssignmentResolve

		beforeEach(() => {
			mockFetch = jest.fn(() => new Promise(r => postAssignmentResolve = r))
			window.fetch = mockFetch

			actual = fixture.postCreateAssignmentRequest({ prop: 'request data' })
		})

		it('should make correct request create assignment request', () => {
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

		describe('when successful response', () => {
			beforeEach(() => {
				postAssignmentResolve(new Response(JSON.stringify({ prop: 'response data' })))
			})

			it('should return data', () => {
				actual.then(data => expect(data).toEqual({ content: { prop: 'response data' } }))
			})
		})

		describe('when failure response', () => {
			beforeEach(() => {
				postAssignmentResolve(new Response(null, { status: 500 }))
			})

			it('should return error', () => {
				actual.then(data => expect(data).toEqual({ error: true }))
			})
		})
	})
})