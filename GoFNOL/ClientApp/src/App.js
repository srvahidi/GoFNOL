import React, { Component } from 'react'
import { Route } from 'react-router'
import { Home } from './components/Home'

export default class App extends Component {

	render() {
		return <React.Fragment>
			<header className="header shadowed">
				<h3>GoFNOL - {this.getEnvironmentName()}</h3>
				<form action="api/user/logout" method="post">
					<button type="submit">Logout</button>
				</form>
			</header>
			<div className="content">
				<Route exact path='/' component={Home} />
			</div>
		</React.Fragment>
	}

	getEnvironmentName = () => {
		const href = this.props.getWindowLocation().href
		if (href.indexOf('-acceptance') !== -1) {
			return 'Acceptance'
		}

		if (href.indexOf('-int') !== -1) {
			return 'Int'
		}

		if (href.indexOf('-demo') !== -1) {
			return 'Demo'
		}

		return 'Local'
	}
}
