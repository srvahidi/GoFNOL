import React from 'react'
import { shallow } from 'enzyme'
import App from './App'

describe('App component', () => {

	let fixture

	beforeEach(() => {
		fixture = shallow(<App getWindowLocation={() => ({ href: 'http://localhost' })} />)
	})

	it('should render local as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Local')
	})
})

describe('App component in acceptance environment', () => {
	let fixture
	beforeEach(() => {
		fixture = shallow(<App getWindowLocation={() => ({ href: 'http://gofnol-acceptance.domain' })} />)
	})

	it('should render acceptance as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Acceptance')
	})
})

describe('App component in int environment', () => {
	let fixture
	beforeEach(() => {
		fixture = shallow(<App getWindowLocation={() => ({ href: 'http://gofnol-pcpm.domain' })} />)
	})

	it('should render int as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - PCPM')
	})
})

describe('App component in demo environment', () => {
	let fixture
	beforeEach(() => {
		fixture = shallow(<App getWindowLocation={() => ({ href: 'http://gofnol-demo.domain' })} />)
	})

	it('should render demo as environment name', () => {
		expect(fixture.find('h3').text()).toBe('GoFNOL - Demo')
	})
})