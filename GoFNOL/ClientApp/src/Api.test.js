import { Api } from './Api'

describe('Api tests', () => {

	let fetch
	let fixture
	let mockAuthService

	beforeEach(() => {
		mockAuthService = {
			getRequestHeaders: () => ({
				testAuthHeader: 'some token'
			})
		}
		fetch = window.fetch
		fixture = new Api(mockAuthService)
	})

	afterEach(() => {
		window.fetch = fetch
	})

	describe('get user data', () => {

		let actual
		let mockFetch
		let getUserDataResolve
		let getUserDataReject

		beforeEach(() => {
			mockFetch = jest.fn(() => new Promise((res, rej) => {
				getUserDataResolve = res
				getUserDataReject = rej
			}))
			window.fetch = mockFetch

			actual = fixture.getUserData('samantha')
		})

		it('should get user data', () => {
			expect(mockFetch).toHaveBeenCalledTimes(1)
			expect(mockFetch).toHaveBeenCalledWith('/api/user/samantha', {
				method: 'GET',
				credentials: 'same-origin',
				headers: {
					testAuthHeader: 'some token'
				}
			})
		})

		describe('when successful response', () => {
			beforeEach(() => {
				getUserDataResolve(new Response(JSON.stringify({ prop: 'data' })))
			})

			it('should resolve with response payload', () => {
				actual.then(data => expect(data).toEqual({ prop: 'data' }))
			})
		})

		describe('when unsuccessful response', () => {
			beforeEach(() => {
				getUserDataResolve(new Response(null, { status: 500 }))
			})

			it('should reject', (done) => {
				actual.catch(() => done())
			})
		})

		describe('when no response', () => {
			beforeEach(() => {
				getUserDataReject()
			})

			it('should reject', (done) => {
				actual.catch(() => done())
			})
		})
	})

	describe('post create assignment request', () => {

		let actual
		let mockFetch
		let postCreateAssignmentResolve
		let postCreateAssignmentReject

		beforeEach(() => {
			mockFetch = jest.fn(() => new Promise((res, rej) => {
				postCreateAssignmentResolve = res
				postCreateAssignmentReject = rej
			}))
			window.fetch = mockFetch

			actual = fixture.postCreateAssignment({ prop: 'request data' })
		})

		it('should make correct request create assignment request', () => {
			expect(mockFetch).toHaveBeenCalledTimes(1)
			expect(mockFetch.mock.calls[0][0]).toBe('/api/fnol')
			expect(mockFetch.mock.calls[0][1]).toEqual({
				method: 'POST',
				credentials: 'same-origin',
				headers: {
					testAuthHeader: 'some token',
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ prop: 'request data' })
			})
		})

		describe('when successful response', () => {
			beforeEach(() => {
				postCreateAssignmentResolve(new Response(JSON.stringify({ prop: 'response data' })))
			})

			it('should resolve with response payload', () => {
				actual.then(data => expect(data).toEqual({ prop: 'response data' }))
			})
		})

		describe('when status 500 response', () => {
			beforeEach(() => {
				postCreateAssignmentResolve(new Response(null, { status: 500 }))
			})

			it('should reject with APIFailure error', (done) => {
				actual.catch(data => {
					expect(data).toEqual(new Error('APIFailure'))
					done()
				})
			})
		})

		describe('when status 502 response', () => {
			beforeEach(() => {
				postCreateAssignmentResolve(new Response(null, { status: 502 }))
			})

			it('should reject with EAIFailure error', (done) => {
				actual.catch(data => {
					expect(data).toEqual(new Error('EAIFailure'))
					done()
				})
			})
		})

		describe('when status 504 response', () => {
			beforeEach(() => {
				postCreateAssignmentResolve(new Response(null, { status: 504 }))
			})

			it('should reject with NetworkFailure error', (done) => {
				actual.catch(data => {
					expect(data).toEqual(new Error('NetworkFailure'))
					done()
				})
			})
		})

		describe('when no response', () => {
			beforeEach(() => {
				postCreateAssignmentReject()
			})

			it('should reject with ClientFailure error', (done) => {
				actual.catch(data => {
					expect(data).toEqual(new Error('ClientFailure'))
					done()
				})
			})
		})
	})
})