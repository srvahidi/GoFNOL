import React from 'react'
import { shallow } from 'enzyme'
import App from './App'

describe('App component', () => {

	let fixture
	let mockApi

	beforeEach(() => {
		mockApi = {
			postUserLogout: jest.fn()
		}
		fixture = shallow(<App api={mockApi} />)
	})

	describe('clicking logout button', () => {
		beforeEach(() => {
			fixture.find('.logout').simulate('click')
		})

		it('should make an api call', () => {
			expect(mockApi.postUserLogout).toHaveBeenCalledTimes(1)
		})
	})

	it('should render local as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Local')
	})
})

describe('App component in acceptance environment', () => {
	let fixture
	beforeEach(() => {
		Object.defineProperty(window.location, 'href', {
			writable: true,
			value: 'http://gofnol-acceptance.domain'
		})
		fixture = shallow(<App />)
	})

	it('should render acceptance as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Acceptance')
	})
})

describe('App component in int environment', () => {
	let fixture
	beforeEach(() => {
		Object.defineProperty(window.location, 'href', {
			writable: true,
			value: 'http://gofnol-int.domain'
		})
		fixture = shallow(<App />)
	})

	it('should render int as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Int')
	})
})

describe('App component in demo environment', () => {
	let fixture
	beforeEach(() => {
		Object.defineProperty(window.location, 'href', {
			writable: true,
			value: 'http://gofnol-demo.domain'
		})
		fixture = shallow(<App />)
	})

	it('should render demo as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Demo')
	})
})