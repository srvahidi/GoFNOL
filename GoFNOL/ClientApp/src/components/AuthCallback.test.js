import React from 'react'
import { shallow } from 'enzyme'

import { AuthCallback } from './AuthCallback'

import * as authService from '../authService'
jest.mock('../authService')

describe('AuthCallback component', () => {
	let fixture
	let mockHistory

	beforeEach(() => {
		mockHistory = {
			push: jest.fn()
		}
		fixture = shallow(<AuthCallback history={mockHistory} />)
	})

	it('should complete sign in and should render nothing', () => {
		expect(fixture.html()).toBe(null)
		expect(authService.completeSignIn).toHaveBeenCalledTimes(1)
		expect(mockHistory.push).toHaveBeenCalledTimes(1)
	})
})
