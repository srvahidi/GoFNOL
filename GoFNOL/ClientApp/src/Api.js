import { getRequestHeaders } from './authService'

export class Api {

	async getUserData(userName) {
		const requestOptions = {
			method: 'GET',
			credentials: 'same-origin',
			headers: getRequestHeaders()
		}

		const response = await fetch(`/api/user/${userName}`, requestOptions)
		if (response.status === 200)
			return await response.json()

		throw new Error()
	}

	async postCreateAssignment(request) {
		const requestOptions = {
			method: 'POST',
			credentials: 'same-origin',
			headers: {
				...getRequestHeaders(),
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(request)
		}

		let response
		try {
			response = await fetch('/api/fnol', requestOptions)
		} catch (e) {
			throw new Error('ClientFailure')
		}

		switch (response.status) {
			case 200:
				return await response.json()
			case 500:
				throw new Error('APIFailure')
			case 502:
				throw new Error('EAIFailure')
			case 504:
				throw new Error('NetworkFailure')
		}
	}
}
