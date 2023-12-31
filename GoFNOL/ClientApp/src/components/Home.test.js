import React from 'react'
import { shallow } from 'enzyme'

import { Home } from './Home.js'

import * as authService from '../authService'
jest.mock('../authService')

describe('Home component when not signed in', () => {
	let fixture

	beforeEach(() => {
		authService.isSignedIn.mockReturnValue(false)
		fixture = shallow(<Home api={{}} />)
	})

	it('should initiate sign in and should render nothing', () => {
		expect(authService.signIn).toHaveBeenCalledTimes(1)
		expect(fixture.html()).toBe(null)
	})
})

describe('Home component when signed in', () => {
	let fixture
	let mockApi
	let postCreateAssignmentResolve
	let postCreateAssignmentReject
	let getUserDataResolve
	let getUserDataReject

	beforeEach(() => {
		jest.useFakeTimers()
		authService.isSignedIn.mockReturnValue(true)
		authService.getUserName.mockReturnValue('samantha')
		mockApi = {
			getUserData: jest.fn(() => new Promise((res, rej) => {
				getUserDataResolve = res
				getUserDataReject = rej
			})),
			postCreateAssignment: jest.fn(() => new Promise((res, rej) => {
				postCreateAssignmentResolve = res
				postCreateAssignmentReject = rej
			}))
		}

		fixture = shallow(<Home api={mockApi} />)
	})

	it('should request user data and render "In Progress" message', () => {
		expect(mockApi.getUserData).toHaveBeenCalledTimes(1)
		expect(mockApi.getUserData).toHaveBeenCalledWith('samantha')
		expect(fixture.find('.status-message').text()).toBe('Requesting NGP data. Please wait.')
		expect(fixture.find('form').exists()).toBe(false)
	})

	describe('when profileId is NOT returned', () => {
		beforeEach(() => {
			getUserDataReject()
		})

		it('should not render form and render unauthorized message', () => {
			expect(fixture.find('.status-message').text()).toBe('GoFNOL is unavailable because there is no Profile ID. Please use the Appraiser\'s credentials for this organization.')
			expect(fixture.find('form').exists()).toBe(false)
		})
	})

	describe('when profileId starting with 477 is returned', () => {
		beforeEach(() => {
			getUserDataResolve({ profileId: '4774PE200001' })
		})

		it('should render profile data', () => {
			expect(fixture.find('.status-message').exists()).toBe(false)
			expect(fixture.find('.profile-label').text()).toBe('Create Claim for Profile: 4774PE200001')
		})

		it('should render all inputs (w/ State Farm branding "Pocket Estimate")', () => {
			const form = fixture.find('.form')

			const mobileFlowIndicator = form.find('.mobile-flow-ind')
			expect(mobileFlowIndicator.find('label').text()).toBe('Mobile Flow Indicator')
			const mobileFlowIndicatorOptions = mobileFlowIndicator.find('option')
			expect(mobileFlowIndicatorOptions.length).toBe(3)
			expect(mobileFlowIndicatorOptions.at(0).text()).toBe('Digital Garage Claims (D)')
			expect(mobileFlowIndicatorOptions.at(1).text()).toBe('Pocket Estimate (Y)')
			expect(mobileFlowIndicatorOptions.at(2).text()).toBe('Not Mobile (N)')

			const claimNumber = form.find('.claim-number')
			expect(claimNumber.find('label').at(0).text()).toBe('New Claim Number')
			expect(claimNumber.find('input[type="text"]').props().placeholder).toBe('Claim Number')

			const generateClaimNumber = claimNumber.find('.generate-claim-number')
			expect(generateClaimNumber.find('span').text()).toBe('Auto Generate')
			expect(generateClaimNumber.find('input.auto-generate-claim').props().checked).toBe(false)

			const firstName = form.find('.first-name')
			expect(firstName.find('label').text()).toBe('First Name')
			expect(firstName.find('input').props().placeholder).toBe('First Name')

			const lastName = form.find('.last-name')
			expect(lastName.find('label').text()).toBe('Last Name')
			expect(lastName.find('input').props().placeholder).toBe('Last Name')

			const phoneNumber = form.find('.phone-number')
			expect(phoneNumber.find('label').text()).toBe('Phone number')
			expect(phoneNumber.find('input').props().placeholder).toBe('Digits Only')

			const zipCode = form.find('.zip-code')
			expect(zipCode.find('label').text()).toBe('ZIP Code')
			expect(zipCode.find('input').props().placeholder).toBe('ZIP Code')

			const city = form.find('.city')
			expect(city.find('label').text()).toBe('City')
			expect(city.find('input').props().placeholder).toBe('City')

			const state = form.find('.state')
			expect(state.find('label').text()).toBe('State/Province')
			expect(state.find('input').props().placeholder).toBe('State/Province')

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

		describe('clicking Create button without entered city value', () => {
			beforeEach(() => {
				const form = fixture.find('.form')
				form.simulate('submit', { preventDefault: jest.fn() })
			})

			it('should NOT make an Api call and should display error message', () => {
				expect(mockApi.postCreateAssignment).toHaveBeenCalledTimes(0)
				expect(fixture.find('.error').text()).toBe('City is a required field. Please update and resubmit.')
			})
		})

		describe('clicking Create button with invalid length phone number', () => {
			beforeEach(() => {
				const form = fixture.find('.form')
				fixture.find('.form .phone-number input').simulate('change', { currentTarget: { value: '012345678' } })
				fixture.find('.form .city input').simulate('change', { currentTarget: { value: 'Cityville' } })
				form.simulate('submit', { preventDefault: jest.fn() })
			})

			it('should NOT make an Api call and should display error message', () => {
				expect(mockApi.postCreateAssignment).toHaveBeenCalledTimes(0)
				expect(fixture.find('.error').text()).toBe('Phone number must be 10 digits. Please update and resubmit.')
			})
		})

		describe('selecting autogenerate claim number', () => {
			beforeEach(() => {
				fixture.find('.form .auto-generate-claim').simulate('change')
			})

			it('should disable claim number input', () => {
				const input = fixture.find('.form .claim-number input[name="claim-number"]')
				expect(input.prop('disabled')).toBe(true)
			})

			describe('filling out all inputs and clicking Create button', () => {
				beforeEach(() => {
					const form = fixture.find('.form')
					form.find('.claim-number input[name="claim-number"]').simulate('change', { currentTarget: { value: 'ABC-123' } })
					form.find('.first-name input').simulate('change', { currentTarget: { value: '1st name' } })
					form.find('.last-name input').simulate('change', { currentTarget: { value: 'nst name' } })
					form.find('.phone-number input').simulate('change', { currentTarget: { value: '(012) 345 67-89' } })
					form.find('.zip-code input').simulate('change', { currentTarget: { value: '34567' } })
					form.find('.city input').simulate('change', { currentTarget: { value: 'Cityville' } })
					form.find('.state input').simulate('change', { currentTarget: { value: 'ST' } })
					form.find('.email input').simulate('change', { currentTarget: { value: 'a@b.c' } })
					form.find('.vin input').simulate('change', { currentTarget: { value: '0123456789ABCDEFG' } })
					form.find('.deductible input.deductible-value').simulate('change', { currentTarget: { value: '500' } })
					form.simulate('submit', { preventDefault: jest.fn() })
				})

				it('should make an Api call', () => {
					expect(mockApi.postCreateAssignment).toHaveBeenCalledTimes(1)

					expect(mockApi.postCreateAssignment.mock.calls[0][0]).toEqual({
						profileId: '4774PE200001',
						mobileFlowIndicator: 'D',
						claimNumber: 'ABC-123',
						owner: {
							firstName: '1st name',
							lastName: 'nst name',
							phoneNumber: '(012)345-6789',
							email: 'a@b.c',
							address: {
								city: 'Cityville',
								zipCode: '34567',
								state: 'ST'
							}
						},
						vin: '0123456789ABCDEFG',
						lossType: 'COLL',
						deductible: '500',
						autoGenerateClaim: true
					})
				})

				it('should disable create button and display elapsed time', () => {
					expect(fixture.find('button.create').props().disabled).toBeTruthy()
					jest.runOnlyPendingTimers()
					expect(fixture.find('.time-elapsed').text()).toBe('Elapsed time: 1 seconds.')
					jest.runOnlyPendingTimers()
					expect(fixture.find('.time-elapsed').text()).toBe('Elapsed time: 2 seconds.')
					jest.runOnlyPendingTimers()
					jest.runOnlyPendingTimers()
					expect(fixture.find('.time-elapsed').text()).toBe('Elapsed time: 4 seconds.')
				})

				describe('when response contains work assignment id', () => {
					beforeEach(() => {
						jest.runOnlyPendingTimers()
						jest.runOnlyPendingTimers()
						jest.runOnlyPendingTimers()
						postCreateAssignmentResolve({
							claimNumber: 'abc-123',
							workAssignmentId: '12345'
						})
					})

					it('should render it and enable create button', () => {
						expect(fixture.find('.work-assignment-id').text()).toBe('Work Assignment ID: \'12345\' added successfully!')
						expect(fixture.find('.time-elapsed').text()).toBe('GoFNOL took 3 seconds to create claim number abc-123')
					})

					describe('clicking Create button again', () => {
						beforeEach(() => {
							fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
							jest.runOnlyPendingTimers()
						})

						it('should hide previous output', () => {
							expect(fixture.find('.time-elapsed').text()).toBe('Elapsed time: 1 seconds.')
							expect(fixture.find('.work-assignment-id').exists()).toBeFalsy()
						})
					})
				})

				const testCases = [
					{ error: 'EAIFailure', message: 'Dependent services failed, please try again. If you continue to experience this error, please contact client services.' },
					{ error: 'NetworkFailure', message: 'GoFNOL failed because it was unable to reach a dependent service, please try again. If you continue to experience this error, please contact client services.' },
					{ error: 'APIFailure', message: 'The API layer encountered an error, please try again. If you continue to experience this error, please contact client services.' },
					{ error: 'ClientFailure', message: 'The client app encountered an error, please try again. If you continue to experience this error, please contact client services.' }
				]

				for (let testCase of testCases) {
					describe(`when response contains ${testCase.error} error`, () => {
						beforeEach(() => {
							jest.runOnlyPendingTimers()
							postCreateAssignmentReject(new Error(testCase.error))
						})

						it('should render it and enable create button', () => {
							expect(fixture.find('.error').text()).toBe(testCase.message)
							expect(fixture.find('button.create').props().disabled).toBeFalsy()
							expect(fixture.find('.time-elapsed').exists()).toBeFalsy()
						})

						describe('clicking Create button again', () => {
							beforeEach(() => {
								fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
								jest.runOnlyPendingTimers()
							})

							it('should hide error', () => {
								expect(fixture.find('.error').exists()).toBeFalsy()
								expect(fixture.find('.time-elapsed').text()).toBe('Elapsed time: 1 seconds.')
							})
						})
					})
				}

			})
		})

		describe('entering lower cased chars as claim number and blurring input focus', () => {
			beforeEach(() => {
				fixture.find('.form .claim-number input[name="claim-number"]')
					.simulate('change', { currentTarget: { value: 'aBc-123-xy' } })
					.simulate('blur')
			})

			it('should uppercase claim number', () => {
				expect(fixture.find('.form .claim-number input[name="claim-number"]').props().value).toBe('ABC-123-XY')
			})
		})

		describe('entering lower cased chars as VIN and blurring input focus', () => {
			beforeEach(() => {
				fixture.find('.form .vin input[name="vin"]')
					.simulate('change', { currentTarget: { value: '0123456789abcdefg' } })
					.simulate('blur')
			})

			it('should uppercase VIN', () => {
				expect(fixture.find('.form .vin input[name="vin"]').props().value).toBe('0123456789ABCDEFG')
			})
		})

		describe('filling out all required fields', () => {
			beforeEach(() => {
				fixture.find('.form .city input').simulate('change', { currentTarget: { value: 'Cityville' } })
			})

			describe('checking waive deductible and clicking Create button', () => {
				beforeEach(() => {
					fixture.find('.form .deductible input.deductible-waive').simulate('change')
					fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
				})

				it('should make an Api call', () => {
					expect(mockApi.postCreateAssignment).toHaveBeenCalledTimes(1)
					expect(mockApi.postCreateAssignment.mock.calls[0][0].deductible).toBe('W')
				})
			})

			describe('selecting "pocket estimate" as mobile flow indicator and clicking Create button', () => {
				beforeEach(() => {
					fixture.find('.form .mobile-flow-ind select').simulate('change', { target: { value: 'Y' } })
					fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
				})

				it('should make an Api call', () => {
					expect(mockApi.postCreateAssignment).toHaveBeenCalledTimes(1)
					expect(mockApi.postCreateAssignment.mock.calls[0][0].mobileFlowIndicator).toBe('Y')
				})
			})

			describe('selecting "not mobile" as mobile flow indicator and clicking Create button', () => {
				beforeEach(() => {
					fixture.find('.form .mobile-flow-ind select').simulate('change', { target: { value: 'N' } })
					fixture.find('.form').simulate('submit', { preventDefault: jest.fn() })
				})

				it('should make an Api call', () => {
					expect(mockApi.postCreateAssignment).toHaveBeenCalledTimes(1)
					expect(mockApi.postCreateAssignment.mock.calls[0][0].mobileFlowIndicator).toBe('N')
				})
			})
		})

		describe('phone number tests', () => {
			describe('entering valid phone number', () => {
				beforeEach(() => {
					fixture.find('.form .phone-number input').simulate('change', { currentTarget: { value: '0123456789' } })
				})

				it('should be valid', () => {
					expect(fixture.find('.form .phone-number input').props().value).toBe('0123456789')
				})
			})

			describe('entering invalid phone number', () => {
				beforeEach(() => {
					fixture.find('.form .phone-number input').simulate('change', { currentTarget: { value: '()-)a9' } })
					fixture.update()
				})

				it('should drop non numeric', () => {
					expect(fixture.find('.form .phone-number input').props().value).toBe('9')
				})
			})
		})
	})

	describe('when profileId that does NOT start with 477 is returned', () => {
		beforeEach(() => {
			getUserDataResolve({ profileId: 'PROF123' })
		})

		it('should render profile data', () => {
			expect(fixture.find('.status-message').exists()).toBe(false)
			expect(fixture.find('.profile-label').text()).toBe('Create Claim for Profile: PROF123')
		})

		it('should render all inputs (w/ GoTime Driver)', () => {
			const form = fixture.find('.form')

			const mobileFlowIndicator = form.find('.mobile-flow-ind')
			expect(mobileFlowIndicator.find('label').text()).toBe('Mobile Flow Indicator')
			const mobileFlowIndicatorOptions = mobileFlowIndicator.find('option')
			expect(mobileFlowIndicatorOptions.length).toBe(3)
			expect(mobileFlowIndicatorOptions.at(0).text()).toBe('Digital Garage Claims (D)')
			expect(mobileFlowIndicatorOptions.at(1).text()).toBe('GoTime Driver (Y)')
			expect(mobileFlowIndicatorOptions.at(2).text()).toBe('Not Mobile (N)')
		})
	})
})
