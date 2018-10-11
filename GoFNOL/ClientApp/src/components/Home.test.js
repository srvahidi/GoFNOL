import React from 'react'
import { shallow } from 'enzyme'
import { Home } from './Home.js'

describe('Home component', () => {

	let fixture
	let mockApi
	let postCreateAssignmentResolve

	beforeEach(() => {
		mockApi = {
			getUserData: jest.fn(() => Promise.resolve({ profileId: 'PROF123' })),
			postCreateAssignmentRequest: jest.fn(() => new Promise(r => postCreateAssignmentResolve = r))
		}

		fixture = shallow(<Home api={mockApi} />)
	})

	it('should render profile data', () => {
		expect(mockApi.getUserData).toHaveBeenCalledTimes(1)
		expect(fixture.find('.profile-label').text()).toBe('Create Claim for Profile: PROF123')
	})

	it('should render all inputs', () => {
		const form = fixture.find('.form')

		const mobileFlowIndicator = form.find('.mobile-flow-ind')
		expect(mobileFlowIndicator.find('label').text()).toBe('Mobile Flow Indicator')
		const mobileFlowIndicatorOptions = mobileFlowIndicator.find('option')
		expect(mobileFlowIndicatorOptions.length).toBe(3)
		expect(mobileFlowIndicatorOptions.at(0).text()).toBe('Digital Garage Claims (D)')
		expect(mobileFlowIndicatorOptions.at(1).text()).toBe('Pocket Estimate (Y)')
		expect(mobileFlowIndicatorOptions.at(2).text()).toBe('Not Mobile (N)')

		const claimNumber = form.find('.claim-number')
		expect(claimNumber.find('label').text()).toBe('New Claim Number')
		expect(claimNumber.find('input').props().placeholder).toBe('Claim Number')

		const firstName = form.find('.first-name')
		expect(firstName.find('label').text()).toBe('First Name')
		expect(firstName.find('input').props().placeholder).toBe('First Name')

		const lastName = form.find('.last-name')
		expect(lastName.find('label').text()).toBe('Last Name')
		expect(lastName.find('input').props().placeholder).toBe('Last Name')

		const phoneNumber = form.find('.phone-number')
		expect(phoneNumber.find('label').text()).toBe('Phone number')
		expect(phoneNumber.find('input').props().placeholder).toBe('Phone number')

		const postalCodes = form.find('.postal-code')
		expect(postalCodes.find('label').text()).toBe('Postal Code')
		expect(postalCodes.find('input.zip-code').props().placeholder).toBe('Zip Code (first 5)')
		expect(postalCodes.find('input.zip-code-extra').props().placeholder).toBe('Extra Zip (extra 4)')

		const state = form.find('.state')
		expect(state.find('label').text()).toBe('State')
		expect(state.find('input').props().placeholder).toBe('State')

		const email = form.find('.email')
		expect(email.find('label').text()).toBe('Email')
		expect(email.find('input').props().placeholder).toBe('Email')

		const vin = form.find('.vin')
		expect(vin.find('label').text()).toBe('VIN')
		expect(vin.find('input').props().placeholder).toBe('VIN')

		const lossType = form.find('.loss-type')
		expect(lossType.find('label').text()).toBe('Loss Type')
		expect(lossType.find('input').props().placeholder).toBe('Loss Type')
		expect(lossType.find('input').props().value).toBe('COLL')

		const deductible = form.find('.deductible')
		expect(deductible.find('label').text()).toBe('Deductible')
		expect(deductible.find('.waive-label').text()).toBe('Waive')
		expect(deductible.find('input.deductible-value').props().placeholder).toBe('Deductible')
	})

	describe('filling out all inputs and clicking Create button', () => {
		beforeEach(() => {
			const form = fixture.find('.form')
			form.find('.claim-number input').simulate('change', { currentTarget: { value: 'ABC-123' } })
			form.find('.first-name input').simulate('change', { currentTarget: { value: '1st name' } })
			form.find('.last-name input').simulate('change', { currentTarget: { value: 'nst name' } })
			form.find('.phone-number input').simulate('change', { currentTarget: { value: '(012) 345 67-89' } })
			form.find('.postal-code input.zip-code').simulate('change', { currentTarget: { value: '34567' } })
			form.find('.postal-code input.zip-code-extra').simulate('change', { currentTarget: { value: '0110' } })
			form.find('.state input').simulate('change', { currentTarget: { value: 'ST' } })
			form.find('.email input').simulate('change', { currentTarget: { value: 'a@b.c' } })
			form.find('.vin input').simulate('change', { currentTarget: { value: '0123456789ABCDEFG' } })
			form.find('.deductible input.deductible-value').simulate('change', { currentTarget: { value: '500' } })
			form.simulate('submit', { preventDefault: jest.fn() })
		})

		it('should make an Api call', () => {
			expect(mockApi.postCreateAssignmentRequest).toHaveBeenCalledTimes(1)
			expect(mockApi.postCreateAssignmentRequest.mock.calls[0][0]).toEqual({
				profileId: 'PROF123',
				mobileFlowIndicator: 'D',
				claimNumber: 'ABC-123',
				owner: {
					firstName: '1st name',
					lastName: 'nst name',
					phoneNumber: '(012) 345 67-89',
					email: 'a@b.c',
					address: {
						postalCode: '34567-0110',
						state: 'ST'
					}
				},
				vin: '0123456789ABCDEFG',
				lossType: 'COLL',
				deductible: '500'
			})
		})

		it('should diable create button', () => {
			expect(fixture.find('button.create').props().disabled).toBeTruthy()
		})

		describe('when response contains work assignment id', () => {
			beforeEach(() => {
				postCreateAssignmentResolve({
					content: {
						workAssignmentId: 12345
					}
				})
			})

			it('should render it and enable create button', () => {
				expect(fixture.find('.work-assignment-id').text()).toBe('Work Assignment ID: \'12345\' added successfully!')
			})

			describe('clicking Create button again', () => {
				beforeEach(() => {
					fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
				})

				it('should hide previous output', () => {
					expect(fixture.find('.work-assignment-id').exists()).toBeFalsy()
				})
			})
		})

		describe('when response contains error', () => {
			beforeEach(() => {
				postCreateAssignmentResolve({
					error: true
				})
			})

			it('should render it and enable create button', () => {
				expect(fixture.find('.error').text()).toBe('GoFNOL failed, please resubmit.')
				expect(fixture.find('button.create').props().disabled).toBeFalsy()
			})

			describe('clicking Create button again', () => {
				beforeEach(() => {
					fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
				})

				it('should hide error', () => {
					expect(fixture.find('.error').exists()).toBeFalsy()
				})
			})
		})
	})

	describe('entering lower cased chars as claim number and bluring input focus', () => {
		beforeEach(() => {
			let form = fixture.find('.form')
			form.find('.claim-number input').simulate('change', { currentTarget: { value: 'aBc-123-xy' } })
			form.find('.claim-number input').simulate('blur')
		})

		it('should uppercase claim number', () => {
			expect(fixture.find('.form .claim-number input').props().value).toBe('ABC-123-XY')
		})
	})

	describe('checking waive deductible and clicking Create button', () => {
		beforeEach(() => {
			fixture.find('.form .deductible input.deductible-waive').simulate('change')
			fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
		})

		it('should make an Api call', () => {
			expect(mockApi.postCreateAssignmentRequest).toHaveBeenCalledTimes(1)
			expect(mockApi.postCreateAssignmentRequest.mock.calls[0][0].deductible).toBe('W')
		})
	})

	describe('selecting "pocket estimate" as mobile flow indicator and clicking Create button', () => {
		beforeEach(() => {
			fixture.find('.form .mobile-flow-ind select').simulate('change', { target: { value: 'Y' } })
			fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
		})

		it('should make an Api call', () => {
			expect(mockApi.postCreateAssignmentRequest).toHaveBeenCalledTimes(1)
			expect(mockApi.postCreateAssignmentRequest.mock.calls[0][0].mobileFlowIndicator).toBe('Y')
		})
	})

	describe('selecting "not mobile" as mobile flow indicator and clicking Create button', () => {
		beforeEach(() => {
			fixture.find('.form .mobile-flow-ind select').simulate('change', { target: { value: 'N' } })
			fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
		})

		it('should make an Api call', () => {
			expect(mockApi.postCreateAssignmentRequest).toHaveBeenCalledTimes(1)
			expect(mockApi.postCreateAssignmentRequest.mock.calls[0][0].mobileFlowIndicator).toBe('N')
		})
	})
})

