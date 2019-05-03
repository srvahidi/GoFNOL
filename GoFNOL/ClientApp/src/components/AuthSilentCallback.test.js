import React from 'react'
import { shallow } from 'enzyme'

import { AuthSilentCallback } from './AuthSilentCallback'

import * as authService from '../authService'
jest.mock('../authService')

describe('AuthSilentCallback component', () => {
	let fixture

	beforeEach(() => {

		fixture = shallow(<AuthSilentCallback />)
	})

	it('should complete sign in and should render nothing', () => {
		expect(authService.completeSilentSignIn).toHaveBeenCalledTimes(1)
		expect(fixture.html()).toBe(null)
	})
})
