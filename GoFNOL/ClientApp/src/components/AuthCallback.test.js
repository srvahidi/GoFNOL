import React from 'react'
import { shallow } from 'enzyme'

import { AuthCallback } from './AuthCallback'

describe('AuthCallback component', () => {
	let fixture
	let mockAuthService

	beforeEach(() => {
		mockAuthService = {
			completeSignIn: jest.fn()
		}
		fixture = shallow(<AuthCallback authService={mockAuthService} />)
	})

	it('should complete sign in and should render nothing', () => {
		expect(mockAuthService.completeSignIn).toHaveBeenCalledTimes(1)
		expect(fixture.html()).toBe(null)
	})
})
