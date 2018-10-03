import React, { Component } from 'react'
import { Api } from '../Api'

import './Home.css'

export class Home extends Component {

	constructor(props) {
		super(props)
		this.api = props.api ? props.api : new Api()
		this.state = {
			mobileFlowIndicator: 'D',
			lossType: 'COLL'
		}
	}

	async componentDidMount() {
		const userData = await this.api.getUserData()
		this.setState({ profileId: userData.profileId })
	}

	render() {
		return (
			<React.Fragment>
				<h2 className="profile-label">Create Claim for Profile: {this.state.profileId}</h2>
				<div className="form">
					<div className="mobile-flow-ind">
						<label>Mobile Flow Indicator</label>
						<select onChange={e => this.setState({ mobileFlowIndicator: e.target.value })} value={this.state.mobileFlowIndicator}>
							<option value={'D'}>Digital Garage Claims (D)</option>
							<option value={'Y'}>Pocket Estimate (Y)</option>
							<option value={'N'}>Not Mobile (N)</option>
						</select>
					</div>
					<div className="claim-number">
						<label>New Claim Number</label>
						<input type="text" placeholder="Claim Number" value={this.state.claimNumber} onChange={e => this.setState({ claimNumber: e.currentTarget.value })} />
					</div>
					<div className="first-name">
						<label>First Name</label>
						<input type="text" placeholder="First Name" value={this.state.firstName} onChange={e => this.setState({ firstName: e.currentTarget.value })} />
					</div>
					<div className="last-name">
						<label>Last Name</label>
						<input type="text" placeholder="Last Name" value={this.state.lastName} onChange={e => this.setState({ lastName: e.currentTarget.value })} />
					</div>
					<div className="phone-number">
						<label>Phone number</label>
						<input type="text" placeholder="Phone number" value={this.state.phoneNumber} onChange={e => this.setState({ phoneNumber: e.currentTarget.value })} />
					</div>
					<div className="postal-code">
						<label>Postal Code</label>
						<input type="text" className="zip-code" placeholder="Zip Code (first 5)" value={this.state.zipCode} onChange={e => this.setState({ zipCode: e.currentTarget.value })} />
						<input type="text" className="zip-code-extra" placeholder="Extra Zip (extra 4)" value={this.state.zipCodeExtra} onChange={e => this.setState({ zipCodeExtra: e.currentTarget.value })} />
					</div>
					<div className="state">
						<label>State</label>
						<input type="text" placeholder="State" value={this.state.state} onChange={e => this.setState({ state: e.currentTarget.value })} />
					</div>
					<div className="email">
						<label>Email</label>
						<input type="text" placeholder="Email" value={this.state.email} onChange={e => this.setState({ email: e.currentTarget.value })} />
					</div>
					<div className="vin">
						<label>VIN</label>
						<input type="text" placeholder="VIN" value={this.state.vin} onChange={e => this.setState({ vin: e.currentTarget.value })} />
					</div>
					<div className="loss-type">
						<label>Loss Type</label>
						<input type="text" placeholder="Loss Type" value={this.state.lossType} onChange={e => this.setState({ lossType: e.currentTarget.value })} />
					</div>
					<div className="deductible">
						<label>Deductible</label>
						<div>
							<span className="waive-label">Waive</span>
							<input type="checkbox" className="deductible-waive" checked={this.state.deductibleWaived} onChange={() => this.setState({ deductibleWaived: !this.state.deductibleWaived })} />
							<input type="text" className="deductible-value" placeholder="Deductible" value={this.state.deductible} onChange={e => this.setState({ deductible: e.currentTarget.value })} />
						</div>
					</div>
				</div>
				<button className="create shadowed" onClick={this.onCreateClicked} disabled={this.state.disableForm}>Create</button>
				{this.state.workAssignmentId && <span className="work-assignment-id">Work Assignment ID: '{this.state.workAssignmentId}' added successfully!</span>}
			</React.Fragment>
		)
	}

	onCreateClicked = async () => {
		const request = {
			profileId: this.state.profileId,
			mobileFlowIndicator: this.state.mobileFlowIndicator,
			claimNumber: this.state.claimNumber,
			owner: {
				firstName: this.state.firstName,
				lastName: this.state.lastName,
				phoneNumber: this.state.phoneNumber,
				email: this.state.email,
				address: {
					postalCode: `${this.state.zipCode}-${this.state.zipCodeExtra}`,
					state: this.state.state
				}
			},
			vin: this.state.vin,
			lossType: this.state.lossType,
			deductible: this.state.deductibleWaived ? 'W' : this.state.deductible
		}
		this.setState({ disableForm: true })
		const response = await this.api.postCreateAssignmentRequest(request)
		this.setState({
			disableForm: false,
			workAssignmentId: response.workAssignmentId
		})
	}
}
