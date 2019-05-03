import React, { Component } from 'react'
import { Route } from 'react-router'

import { Api } from './Api'
import { signOut } from './authService'

import { Home } from './components/Home'
import { AuthSilentCallback } from './components/AuthSilentCallback'
import { AuthCallback } from './components/AuthCallback'

export default class App extends Component {

	render() {
		return <React.Fragment>
			<header className="header shadowed">
				<h3>GoFNOL - {this.getEnvironmentName()}</h3>
				<button onClick={signOut}>Logout</button>
			</header>
			<div className="content">
				<Route exact path='/' render={props => <Home {...props} api={new Api()} environment={this.getEnvironmentName()} />} />
				<Route path='/auth-callback' render={props => <AuthCallback {...props} />} />
				<Route path='/auth-silent-callback' render={props => <AuthSilentCallback {...props} />} />
			</div>
		</React.Fragment>
	}

	getEnvironmentName = () => {
		const href = this.props.getWindowLocation().href
		if (href.indexOf('-acceptance') !== -1) {
			return Environment.Acceptance
		}

		if (href.indexOf('-int') !== -1) {
			return Environment.Int
		}

		if (href.indexOf('-demo') !== -1) {
			return Environment.Demo
		}

		return Environment.Local
	}
}

export const Environment = {
	Local: 'Local',
	Acceptance: 'Acceptance',
	Int: 'Int',
	Demo: 'Demo'
}