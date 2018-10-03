import React, { Component } from 'react'
import { Route } from 'react-router'
import { Home } from './components/Home'
import { Api } from './Api'

export default class App extends Component {

	constructor(props) {
		super(props)
		this.api = props.api ? props.api : new Api()
		this.state = {}
	}

	render() {
		return <React.Fragment>
			<header className="header shadowed">
				<h3>GoFNOL - {this.getEnvironmentName()}</h3>
				<button className="logout" onClick={this.onLogoutClick}>Logout</button>
			</header>
			<div className="content">
				<Route exact path='/' component={Home} />
			</div>
		</React.Fragment>
	}

	getEnvironmentName = () => {
		if (window.location.href.indexOf('-acceptance') !== -1) {
			return 'Acceptance'
		}

		if (window.location.href.indexOf('-int') !== -1) {
			return 'Int'
		}

		if (window.location.href.indexOf('-demo') !== -1) {
			return 'Demo'
		}

		return 'Local'
	}

	onLogoutClick = () => {
		this.api.postUserLogout()
	}
}
