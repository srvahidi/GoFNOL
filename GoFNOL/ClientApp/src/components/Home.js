import React, { Component } from 'react'

import { isSignedIn, signIn, getUserName } from '../authService'

import './Home.css'

export class Home extends Component {

	errorMessages = {
		EAIFailure: 'Dependent services failed, please try again. If you continue to experience this error, please contact client services.',
		NetworkFailure: 'GoFNOL failed because it was unable to reach a dependent service, please try again. If you continue to experience this error, please contact client services.',
		APIFailure: 'The API layer encountered an error, please try again. If you continue to experience this error, please contact client services.',
		ClientFailure: 'The client app encountered an error, please try again. If you continue to experience this error, please contact client services.'
	}

	constructor(props) {
		super(props)
		this.state = {
			mobileFlowIndicator: 'D',
			claimNumber: '',
			firstName: '',
			lastName: '',
			phoneNumber: '',
			zipCode: '',
			city: '',
			state: '',
			email: '',
			vin: '',
			lossType: 'COLL',
			deductibleWaived: false,
			deductible: '',
			autoGenerateClaim: false,
			createdClaimNumber: ''
		}
	}

	async componentDidMount() {
		if (!isSignedIn()) {
			signIn()
			return
		}

		try {
			const userDataResponse = await this.props.api.getUserData(getUserName())
			this.setState({ profileId: userDataResponse.profileId })
		} catch (e) {
			this.setState({ profileId: null })
		}
	}

	render() {
		if (!isSignedIn()) {
			return null
		}

		const { profileId } = this.state

		if (typeof profileId === 'undefined') {
			return <div className="status-message">Requesting NGP data. Please wait.</div>
		}

		if (profileId === null) {
			return <div className="status-message">GoFNOL is unavailable because there is no Profile ID. Please use the Appraiser's credentials for this organization.</div>
		}

		const isStateFarm = profileId.toString().trim().toLowerCase().startsWith('477')

		return (
			<React.Fragment>
				<h2 className="profile-label">Create Claim for Profile: {profileId}</h2>
				<form className="form" onSubmit={this.onFormSubmit}>
					<div className="mobile-flow-ind">
						<label>Mobile Flow Indicator</label>
						<select onChange={e => this.setState({ mobileFlowIndicator: e.target.value })} value={this.state.mobileFlowIndicator}>
							<option value={'D'}>Digital Garage Claims (D)</option>
							<option value={'Y'}>{isStateFarm ? 'Pocket Estimate (Y)' : 'GoTime Driver (Y)'}</option>
							<option value={'N'}>Not Mobile (N)</option>
						</select>
					</div>
					<div className="claim-number">
						<label>New Claim Number</label>
						<div className="generate-claim-number">
							<span>Auto Generate</span>
							<input type="checkbox" checked={this.state.autoGenerateClaim} className="auto-generate-claim" onChange={e => this.setState({ autoGenerateClaim: !this.state.autoGenerateClaim })} />
							<input type="text" name="claim-number" placeholder="Claim Number" value={this.state.claimNumber} disabled={this.state.autoGenerateClaim}
								onChange={e => this.setState({ claimNumber: e.currentTarget.value })}
								onBlur={() => this.setState({ claimNumber: this.state.claimNumber.toUpperCase() })} />
						</div>
					</div>
					<div className="first-name">
						<label>First Name</label>
						<input type="text" name="first-name" placeholder="First Name" value={this.state.firstName} onChange={e => this.setState({ firstName: e.currentTarget.value })} />
					</div>
					<div className="last-name">
						<label>Last Name</label>
						<input type="text" name="last-name" placeholder="Last Name" value={this.state.lastName} onChange={e => this.setState({ lastName: e.currentTarget.value })} />
					</div>
					<div className="phone-number">
						<label>Phone number</label>
						<input type="text" name="phone-number" placeholder="Digits Only" value={this.state.phoneNumber} onChange={e => this.setState({ phoneNumber: e.currentTarget.value.replace(/\D/g,'') }) } />
					</div>
					<div className="zip-code">
						<label>ZIP Code</label>
						<input type="text" name="zip-code" placeholder="ZIP Code" value={this.state.zipCode} onChange={e => this.setState({ zipCode: e.currentTarget.value })} />
					</div>
					<div className="city">
						<label>City</label>
						<input type="text" name="city" placeholder="City" value={this.state.city} onChange={e => this.setState({ city: e.currentTarget.value })} />
					</div>
					<div className="state">
						<label>State/Province</label>
						<input type="text" name="state" placeholder="State/Province" value={this.state.state} onChange={e => this.setState({ state: e.currentTarget.value })} />
					</div>
					<div className="email">
						<label>Email</label>
						<input type="text" name="email" placeholder="Email" value={this.state.email} onChange={e => this.setState({ email: e.currentTarget.value })} />
					</div>
					<div className="vin">
						<label>VIN</label>
						<input type="text" name="vin" placeholder="VIN" value={this.state.vin} onChange={e => this.setState({ vin: e.currentTarget.value })} onBlur={() => this.setState({ vin: this.state.vin.toUpperCase() })} />
					</div>
					<div className="loss-type">
						<label>Loss Type</label>
						<input type="text" name="loss-type" placeholder="Loss Type" value={this.state.lossType} onChange={e => this.setState({ lossType: e.currentTarget.value })} />
					</div>
					<div className="deductible">
						<label>Deductible</label>
						<div>
							<span className="waive-label">Waive</span>
							<input type="checkbox" className="deductible-waive" checked={this.state.deductibleWaived} onChange={() => this.setState({ deductibleWaived: !this.state.deductibleWaived })} />
							<input type="text" name="deductible" className="deductible-value" placeholder="Deductible" value={this.state.deductible} onChange={e => this.setState({ deductible: e.currentTarget.value })} />
						</div>
					</div>
					<button type="submit" className="create shadowed" disabled={this.state.inProgress}>Create</button>
				</form>
				{this.state.stopwatchSeconds > 0 && <span className="time-elapsed">{this.state.inProgress ? `Elapsed time: ${this.state.stopwatchSeconds} seconds.` : `GoFNOL took ${this.state.stopwatchSeconds} seconds to create claim number ${this.state.createdClaimNumber}`}</span>}
				{this.state.errorMessage && <span className="error">{this.state.errorMessage}</span>}
				{this.state.workAssignmentId && <span className="work-assignment-id">Work Assignment ID: '{this.state.workAssignmentId}' added successfully!</span>}
			</React.Fragment>
		)
	}

	onFormSubmit = (e) => {
		this.submitRequest()
		e.preventDefault()
	}

	submitRequest = async () => {
		const request = {
			profileId: this.state.profileId,
			mobileFlowIndicator: this.state.mobileFlowIndicator,
			claimNumber: this.state.claimNumber,
			owner: {
				firstName: this.state.firstName,
				lastName: this.state.lastName,
				phoneNumber: '(' + this.state.phoneNumber.slice(0, 3) + ')' + this.state.phoneNumber.slice(3, 6) + '-' + this.state.phoneNumber.slice(6),
				email: this.state.email,
				address: {
					city: this.state.city,
					zipCode: this.state.zipCode,
					state: this.state.state
				}
			},
			vin: this.state.vin,
			lossType: this.state.lossType,
			deductible: this.state.deductibleWaived ? 'W' : this.state.deductible,
			autoGenerateClaim: this.state.autoGenerateClaim
		}

		if (!request.owner.address.city) {
			this.setState({ errorMessage: 'City is a required field. Please update and resubmit.' })
			return
		}

		if (this.state.phoneNumber && this.state.phoneNumber.length !== 10) {
			this.setState({ errorMessage: 'Phone number must be 10 digits. Please update and resubmit.' })
			return
		}

		this.setState({
			inProgress: true,
			stopwatchSeconds: 0,
			errorMessage: '',
			createdClaimNumber: '',
			workAssignmentId: ''
		})
		const interval = setInterval(() => {
			this.setState(s => {
				return {
					stopwatchSeconds: s.stopwatchSeconds ? s.stopwatchSeconds + 1 : 1
				}
			})
		}, 1000)
		try {
			const response = await this.props.api.postCreateAssignment(request)
			this.setState({
				inProgress: false,
				createdClaimNumber: response.claimNumber,
				workAssignmentId: response.workAssignmentId
			})
		}
		catch (error) {
			this.setState({
				inProgress: false,
				stopwatchSeconds: 0,
				errorMessage: this.errorMessages[error.message]
			})
		}
		clearInterval(interval)
	}
}
